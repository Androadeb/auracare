using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ynex.Models.Auth;
using ynex.Models.Chat;
using ynex.Models.MedicalTest;
using ynex.Models.Patient;
using ynex.Models.Treatment;
using ynex.Services.Chat;
using ynex.Services.MedicalTest;
using ynex.Services.Patient;
using ynex.Services.Treatment;

namespace ynex.Pages.Admin.Patient_Info
{
    public partial class Patient_Info : ComponentBase
    {
        // ==============================================================================================
        // SECTION 1: CORE & PATIENT INFO (Properties, Auth, Initial Load)
        // ==============================================================================================

        [Parameter] public string Id { get; set; }
        [Inject] public HttpClient Http { get; set; }
        [Inject] public NavigationManager Nav { get; set; }
        [Inject] public IPatientService PatientDataService { get; set; }
        [Inject] protected IJSRuntime JS { get; set; }
        [Inject] public AuthState Auth { get; set; } = default!;
        protected PatientSessionItem? session;
        protected bool isLoading = true;
        protected string? errorMessage;
        private string activeTab = "info";
        protected bool isSubmitting = false;
       
        public List<MedicalTestOrderDetail> CompletedOrders { get; set; } = new();
        private string? _accessToken;
        private string? _refreshToken => Auth?.RefreshToken;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoading = true;
                _accessToken = Auth.AccessToken; // يمكن تركه إذا كانت الخدمات الأخرى (مثل PatientDataService) لا تزال تحتاجه

                // 1. تحديث الاستدعاءات: حذف _accessToken من خدمات التحاليل (TestService)
                var patientTask = PatientDataService.GetPatientDetailsAsync(Id, _accessToken);
                var chatTask = ChatService.GetChatMessagesAsync(Id, _accessToken);

                // هنا حذفنا _accessToken
                var testsTask = TestService.GetOrdersBySessionIdAsync(Id);

                await LoadCurrentAddedItems();
                await Task.WhenAll(patientTask, chatTask, testsTask);
                await LoadData();
                session = await patientTask;
                var initialMessages = await chatTask ?? new List<ChatMessage>();

                foreach (var msg in initialMessages)
                {
                    msg.IsMe = (msg.SenderId == currentUserId);
                }
                messages = initialMessages;

                var allTests = await testsTask;
                if (allTests != null && allTests.Any())
                {
                    
                    var detailTasks = allTests.Select(test => TestService.GetOrderDetailsAsync(test.id)).ToList();
                    var detailsResults = await Task.WhenAll(detailTasks);

                    CompletedOrders.Clear();
                    foreach (var detail in detailsResults)
                    {
                        if (detail != null && !string.IsNullOrEmpty(detail.resultFileUrl) && detail.diagnosticSessionId == Id)
                        {
                            CompletedOrders.Add(detail);
                        }
                    }
                }

                await StartHubConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error]: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
        private async Task LoadPatientDetails()
        {
            try
            {
                isLoading = true;
                errorMessage = null;

                // استدعاء السيرفس
                var result = await PatientDataService.GetPatientDetailsAsync(Id, _accessToken);

                if (result != null)
                {
                    session = result;
                }
                else
                {
                    errorMessage = "لم يتم العثور على بيانات الجلسة.";
                }
            }
            catch (UnauthorizedAccessException)
            {
                var isRefreshed = await HandleTokenRefresh();
                if (isRefreshed) await LoadPatientDetails();
                else Nav.NavigateTo("/auth/sign-in");
            }
            catch (Exception ex)
            {
                errorMessage = "حدث خطأ: " + ex.Message;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
        private async Task<bool> HandleTokenRefresh()
        {
            var authData = await PatientDataService.RefreshTokenAsync(_refreshToken);
            if (authData != null)
            {
                Auth.AccessToken = authData.AccessToken;
                Auth.RefreshToken = authData.RefreshToken;
                return true;
            }
            return false;
        }

        protected string GetStatusStyle(string status)
        {
            if (string.IsNullOrEmpty(status)) return "background-color: #f1f5f9; color: #64748b;";

            // تحويل النص لحروف صغيرة والمقارنة بالرقم أو الكلمة
            return status.ToLower().Trim() switch
            {
                "0" or "pending" => "background-color: #fef3c7 !important; color: #d97706 !important;",
                "1" or "received" or "new" => "background-color: #e0faff !important; color: #0ea5e9 !important;",
                "2" or "completed" or "complete" => "background-color: #dcfce7 !important; color: #15803d !important;",
                "3" or "inprogress" => "background-color: #fff7ed !important; color: #c2410c !important;",
                _ => "background-color: #f1f5f9; color: #64748b;"
            };
        }

        private async Task HandleCompleteSession()
        {
            if (isSubmitting) return;

            try
            {
                isSubmitting = true;
                var success = await PatientDataService.CompleteSessionAsync(Id, _accessToken);

                if (success)
                {
                    if (session != null)
                    {
                        session.status = "Completed";
                    }

                    // إشعار عصري باللون الأخضر
                    await JS.InvokeVoidAsync("Swal.fire", new
                    {
                        title = Loc["Success"],
                        text = Loc["SessionCompletedSuccessfully"],
                        icon = "success",
                        confirmButtonColor = "#F1A68E", // نفس لون أزرارك ليكون التصميم متناسق
                        timer = 2000, // يختفي تلقائياً بعد ثانيتين
                        showConfirmButton = false,
                        position = "center",
                        toast = false // يمكنك جعلها true إذا أردت إشعاراً صغيراً في الزاوية (Toast)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error completing session: {ex.Message}");
            }
            finally
            {
                isSubmitting = false;
                StateHasChanged();
            }
        }
        protected int? PatientAge
        {
            get
            {
                
                if (session?.dateOfBirth == null) return null;

               
                DateTime birthDate = session.dateOfBirth.Value;

                var today = DateTime.Now;
                var age = today.Year - birthDate.Year;

               
                if (birthDate.Date > today.AddYears(-age))
                {
                    age--;
                }

                return age;
            }
        }

        // ==============================================================================================
        // SECTION 2: CHAT LOGIC
        // ==============================================================================================
        [Inject] public IChatService ChatService { get; set; }

        protected List<ChatMessage> messages = new();
        protected string newMessageText = "";
        protected bool isSending = false;
        protected bool showChat = false;
        protected bool isChatMenuOpen = false;
        protected string? selectedImageUrl;
        protected IBrowserFile? selectedFile;
        private HubConnection? hubConnection;
        protected string currentUserId = "231e3e6e-3ff8-4bc1-9f16-9c3c0f4b38af";
        private bool isMeetingActive = false;
        private bool isDoctorScheduleModalOpen = false;
        private List<DoctorScheduleDto> doctorSchedules = new();

        protected void ToggleChatMenu() => isChatMenuOpen = !isChatMenuOpen;
        protected void EnableVideoConsultation() { isChatMenuOpen = false; Console.WriteLine("Video Consultation Enabled"); }
        protected void OpenImage(string url) { selectedImageUrl = url; StateHasChanged(); }
        protected void CloseImage() { selectedImageUrl = null; StateHasChanged(); }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // 1. جلب كائن اليوزر بالكامل كما يخزنه الـ AuthService
                var userDataJson = await JS.InvokeAsync<string>("localStorage.getItem", "user_data");

                if (!string.IsNullOrEmpty(userDataJson))
                {
                    try
                    {
                        // 2. تحويل النص إلى كائن (أو استخدام الـ Model الخاص بك LoginResponse)
                        var userData = JsonSerializer.Deserialize<ynex.Models.Auth.LoginResponse>(userDataJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (userData != null && !string.IsNullOrEmpty(userData.AccessToken))
                        {
                            _accessToken = userData.AccessToken;

                            // 3. الآن نبدأ الاتصال بالتوكن الصحيح
                            await StartHubConnection(_accessToken);

                            // تحميل الرسائل
                            messages = await ChatService.GetChatMessagesAsync(Id, _accessToken);

                            StateHasChanged();
                            _ = ScrollToBottom();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing user_data: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("SignalR: user_data not found in localStorage.");
                }
            }
        }
        private async Task OpenScheduleModal()
        {
            isDoctorScheduleModalOpen = true;
            var data = await ChatService.GetSchedulesAsync(_accessToken);
            if (data != null)
            {
                // ترتيب الأيام لتبدأ من الأحد مثلاً أو السبت حسب رغبتك
                doctorSchedules = data.OrderBy(x => x.DayOfWeek).ToList();
            }
        }
        private string GetDayName(DayOfWeek dayOfWeek)
        {
            // يحول رقم اليوم لاسم اليوم حسب لغة الجهاز (الجمعة، السبت، إلخ)
            return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dayOfWeek);
        }

        private async Task SaveSchedules()
        {
            // سنقوم بإنشاء نسخة جديدة من المواعيد محولة لـ UTC قبل الإرسال
            var schedulesToSave = doctorSchedules.Select(s => new DoctorScheduleDto
            {
                Id = s.Id,
                DayOfWeek = s.DayOfWeek,
                IsAvailable = s.IsAvailable,
                // هنا السحر: نحول الـ TimeSpan المحلي إلى UTC
                // نفترض أننا سنستخدم توقيت الجهاز المتصفح حالياً
                StartTime = ConvertToUtc(s.StartTime),
                EndTime = ConvertToUtc(s.EndTime)
            }).ToList();

            var success = await ChatService.UpdateSchedulesAsync(schedulesToSave, _accessToken);

            if (success)
            {
                isDoctorScheduleModalOpen = false;
             
                await LoadData();
            }
        }
        private TimeSpan ConvertToUtc(TimeSpan localTime)
        {
            // دمج الوقت مع تاريخ اليوم للحصول على DateTime كامل
            DateTime localDateTime = DateTime.Today.Add(localTime);

            // التحويل لـ UTC بناءً على توقيت جهاز الدكتور حالياً
            DateTime utcDateTime = localDateTime.ToUniversalTime();

            return utcDateTime.TimeOfDay;
        }

        private TimeSpan ConvertToLocal(TimeSpan utcTime)
        {
            // اعتبار الوقت القادم من السيرفر UTC
            DateTime utcDateTime = DateTime.SpecifyKind(DateTime.Today.Add(utcTime), DateTimeKind.Utc);

            // التحويل لتوقيت الجهاز المحلي (Local)
            return utcDateTime.ToLocalTime().TimeOfDay;
        }


        private void DismissDoctorScheduleModal()
        {
            isDoctorScheduleModalOpen = false;
        }
        private async Task LoadData()
        {
            var data = await ChatService.GetSchedulesAsync(_accessToken);
            if (data != null && data.Any())
            {
                doctorSchedules = data;
            }
        }
        private async Task StartHubConnection(string? token = null)
        {
            // 1. تحديد التوكن المستخدم (الممرر للدالة أو المخزن في الكلاس)
            var connectionToken = token ?? _accessToken;

            if (string.IsNullOrWhiteSpace(connectionToken))
            {
                Console.WriteLine("SignalR Error: Cannot start connection without an Access Token.");
                return;
            }

            try
            {
                // 2. التحقق من حالة الاتصال الحالي لتجنب التكرار
                if (hubConnection != null && hubConnection.State != HubConnectionState.Disconnected)
                    return;

                // 3. إنشاء الاتصال باستخدام الخدمة
                hubConnection = ChatService.CreateHubConnection(connectionToken);

                // 4. تعريف استقبال الرسائل (ReceiveMessage)
                hubConnection.On<ChatMessage>("ReceiveMessage", async (message) =>
                {
                    // التحقق من أن الرسالة تخص هذه الجلسة وأنها ليست مكررة
                    if (message.DiagnosticSessionId == Id && !messages.Any(m => m.Id == message.Id))
                    {
                        message.IsMe = (message.SenderId == currentUserId);

                        // منطق التحديث الذكي (Optimistic UI Matching)
                        var existingMsg = messages.FirstOrDefault(m =>
                            m.SenderId == currentUserId &&
                            m.Message == message.Message &&
                            m.Id.Length > 30); // التمييز بين GUID المؤقت والدائم

                        if (existingMsg != null)
                        {
                            existingMsg.Id = message.Id;
                            existingMsg.Token = message.Token;
                            existingMsg.RoomId = message.RoomId;
                            existingMsg.FileUrl = message.FileUrl;
                            existingMsg.Type = message.Type;
                        }
                        else
                        {
                            messages.Add(message);
                        }

                        await InvokeAsync(() =>
                        {
                            StateHasChanged();
                            _ = ScrollToBottom();
                        });
                    }
                });

                // 5. تشغيل الاتصال
                await hubConnection.StartAsync();

                // 6. الانضمام لمجموعة الجلسة فور الاتصال بنجاح
                if (!string.IsNullOrEmpty(Id))
                {
                    // تأكد من استخدام الاسم الصحيح للميثود في السيرفر (JoinSessionGroupAsync)
                    await hubConnection.InvokeAsync("JoinSessionGroupAsync", Id);
                    Console.WriteLine($"SignalR: Successfully joined group {Id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR Connection Error: {ex.Message}");
            }
        }

       
      

        private async Task SendMessage()
        {
            if (session?.status?.ToLower() == "completed" || session?.status == "2")
            {
                return;
            }
            if (isSending) return;
            if (string.IsNullOrWhiteSpace(newMessageText) && selectedFile == null) return;

            isSending = true;
            var tempId = Guid.NewGuid().ToString();

            try
            {
                // 1. خزن القيم في متغيرات مؤقتة فوراً قبل التصفير
                var textToPayload = newMessageText?.Trim() ?? "";
                var fileToPayload = selectedFile;

                // 2. تحديث الواجهة للمستخدم (Optimistic UI)
                var tempMsg = new ChatMessage
                {
                    Id = tempId,
                    Message = textToPayload,
                    SenderId = currentUserId,
                    IsMe = true,
                    CreationTime = DateTime.Now,
                    // لو فيه ملف، ممكن تضيف منطق عرض صورة مؤقتة هنا لو حابب
                };

                messages.Add(tempMsg);

                // تصفير الحقول الآن آمن لأننا حفظنا القيم في الـ local variables
                newMessageText = "";
                selectedFile = null;

                StateHasChanged();
                _ = ScrollToBottom();

                // 3. بناء الـ Payload باستخدام المتغيرات المؤقتة
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(Id), "DiagnosticSessionId");

                if (!string.IsNullOrEmpty(textToPayload))
                {
                    content.Add(new StringContent(textToPayload), "Message");
                }

                if (fileToPayload != null)
                {
                    var stream = fileToPayload.OpenReadStream(maxAllowedSize: 1024 * 1024 * 10); // 10MB
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(fileToPayload.ContentType);
                    content.Add(fileContent, "File", fileToPayload.Name);
                }

                // 4. الإرسال
                var result = await ChatService.SendChatMessageAsync(content, _accessToken);

                if (result != null)
                {
                    tempMsg.Id = result.Id;
                    tempMsg.FileUrl = result.FileUrl; // تحديث رابط الملف بعد الرفع
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send Error: {ex.Message}");
                // ممكن ترجع الرسالة للـ input لو فشل الإرسال تماماً
            }
            finally
            {
                isSending = false;
                StateHasChanged();
            }
        }
        private async Task ScrollToBottom()
        {
            try { await Task.Delay(100); await JS.InvokeVoidAsync("scrollToBottom", "chat-container"); }
            catch { }
        }

        private void HandleKeyUp(KeyboardEventArgs e) { if (e.Key == "Enter") _ = SendMessage(); }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }
        private bool isMicOn = true;
   

      
        

     
        private async Task JoinMeeting(ChatMessage message)
        {
            if (string.IsNullOrEmpty(message.RoomId) || string.IsNullOrEmpty(message.Token))
            {
                Console.WriteLine("Meeting data is missing.");
                return;
            }

            // تفعيل الواجهة أولاً لإنشاء الـ div في الـ DOM
            isMeetingActive = true;
            StateHasChanged();

            // تأخير بسيط لضمان أن المتصفح تعرف على الـ div الجديد
            await Task.Delay(500);

            try
            {
                // استدعاء الـ JS
                await JS.InvokeVoidAsync("videoSDKHandler.init", message.RoomId, message.Token, "Doctor");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        // ==============================================================================================
        // SECTION 3: MEDICAL TEST LOGIC - Corrected
        // ==============================================================================================
        [Inject] public IMedicalTestService TestService { get; set; }
        public MedicalTestOrderDetail? OrderDetail { get; set; }
        private bool isModalVisible = false;
        protected List<MedicalCategory> categories = new();
        protected bool isCategoriesLoading = false;
       
        // تم توحيد اسم المتغير هنا لإصلاح خطأ "The name does not exist"
        protected string? selectedCategoryId { get; set; }

        protected List<SampleTypeItem> sampleTypes = new();
        protected bool isSamplesLoading = false;
        protected List<MedicalTestItem> testNames = new();
        protected bool isTestsLoading = false;
        public List<TestResultDetail> CompletedTests { get; set; } = new ();
        protected CreateMedicalTestRequest testModel = new();

        private void CloseModal() => isModalVisible = false;
       

     
        private async Task OpenModal()
        {
            isModalVisible = true;
            testModel = new();
            sampleTypes = new();
            testNames = new();

            if (!categories.Any())
            {
                await LoadCategories();
            }
            StateHasChanged();
        }

        private async Task LoadCategories()
        {
            try
            {
                isCategoriesLoading = true;
                
                categories = await TestService.GetCategoriesAsync();
            }
            catch (Exception ex) { Console.WriteLine($"Error loading categories: {ex.Message}"); }
            finally { isCategoriesLoading = false; StateHasChanged(); }
        }

        private async Task OnCategoryChanged(ChangeEventArgs e)
        {
            selectedCategoryId = e.Value?.ToString();

            testNames = new();
            sampleTypes = new();
            testModel.MedicalTestId = null;
            testModel.SampleTypeId = null;

            if (!string.IsNullOrEmpty(selectedCategoryId))
            {
                try
                {
                    isTestsLoading = true;
                    // تم حذف _accessToken
                    testNames = await TestService.GetTestsByCategoryAsync(selectedCategoryId);
                }
                catch (Exception ex) { Console.WriteLine($"Error loading tests: {ex.Message}"); }
                finally { isTestsLoading = false; StateHasChanged(); }
            }
        }

        private async Task OnTestChanged(ChangeEventArgs e)
        {
            var testId = e.Value?.ToString();
            testModel.MedicalTestId = testId;
            testModel.SampleTypeId = null;
            sampleTypes = new();

            if (!string.IsNullOrEmpty(testId))
            {
                try
                {
                    isSamplesLoading = true;
                    // تم حذف _accessToken
                    sampleTypes = await TestService.GetSampleTypesByTestIdAsync(testId);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                finally { isSamplesLoading = false; StateHasChanged(); }
            }
        }

        private async Task HandleCreateTest()
        {
            if (string.IsNullOrEmpty(testModel.MedicalTestId) || string.IsNullOrEmpty(testModel.SampleTypeId)) return;

            try
            {
                isSubmitting = true;

                testModel.DiagnosticSessionId = Id;

                // تم حذف _accessToken من هنا لأن الخدمة تسحبه الآن تلقائياً
                var newTestMessage = await TestService.CreateMedicalTestAsync(testModel);

                if (newTestMessage != null)
                {
                    messages.Add(newTestMessage);

                    _ = ScrollToBottom();
                    CloseModal();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                isSubmitting = false;

                StateHasChanged();
            }
        }



        // ==============================================================================================
        // SECTION 4: TREATMENT PLAN LOGIC - Verified & Final
        // ==============================================================================================
        [Inject] public ITreatmentService TreatmentService { get; set; }

        bool isTreatmentModalVisible = false;
        bool isMedicationsLoading = false;
        List<MedicationItem> medicationsList = new();
        private List<TreatmentPlanDetail> currentAddedItems = new();
        private TreatmentPlanDetail treatmentModel = new TreatmentPlanDetail();

      

        async Task OpenTreatmentModal()
        {
            treatmentModel = new TreatmentPlanDetail { Frequency = "once Daily" };
            isTreatmentModalVisible = true;
            if (!medicationsList.Any()) await LoadMedications();
        }

        void CloseTreatmentModal() => isTreatmentModalVisible = false;

        private async Task LoadMedications()
        {
            try
            {
                isMedicationsLoading = true;
                medicationsList = await TreatmentService.GetMedicationsAsync(_accessToken);
            }
            catch (Exception ex) { Console.WriteLine($"Medication Load Error: {ex.Message}"); }
            finally { isMedicationsLoading = false; StateHasChanged(); }
        }

        private void OnMedicationChanged(ChangeEventArgs e)
        {
            var selectedName = e.Value?.ToString();
            var selectedMed = medicationsList.FirstOrDefault(m => m.Name == selectedName);

            if (selectedMed != null)
            {
                treatmentModel.MedicationId = selectedMed.Id;
                treatmentModel.MedicationName = selectedMed.Name;
                treatmentModel.Dosage = selectedMed.Dosage;
            }
            else
            {
                treatmentModel.MedicationId = null;
                treatmentModel.MedicationName = selectedName;
            }
        }

        async Task HandleCreateTreatmentPlan()
        {
            if (string.IsNullOrEmpty(treatmentModel.MedicationId)) return;

            try
            {
                isSubmitting = true;

                // PostData مطابق تماماً للـ Request Body في الـ Swagger
                var postData = new
                {
                    diagnosticSessionId = Id,
                    medicationId = treatmentModel.MedicationId,
                    dosage = treatmentModel.Dosage,
                    frequency = treatmentModel.Frequency,
                    duration = treatmentModel.Duration,
                    quantity = treatmentModel.Quantity,
                    refills = treatmentModel.Refills,
                    specialInstructions = treatmentModel.SpecialInstructions
                };

                var success = await TreatmentService.AddTreatmentItemAsync(postData, _accessToken);

                if (success)
                {
                    await LoadCurrentAddedItems();
                    CloseTreatmentModal();
                    treatmentModel = new TreatmentPlanDetail();
                }
            }
            catch (Exception ex) { Console.WriteLine($"Add Item Error: {ex.Message}"); }
            finally { isSubmitting = false; StateHasChanged(); }
        }

        private async Task LoadCurrentAddedItems()
        {
            if (string.IsNullOrEmpty(Id)) return;
            try
            {
                // استدعاء الخدمة (تأكد أن المسار في الخدمة تم تحديثه لـ added-items/{sessionId})
                currentAddedItems = await TreatmentService.GetAddedItemsAsync(Id, _accessToken);
            }
            catch (Exception ex) { Console.WriteLine($"Load Drafts Error: {ex.Message}"); }
            finally { StateHasChanged(); }
        }

        private async Task HandleDeleteItem(string? itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return;

            try
            {
                // itemId هنا هو المعرف الفريد لعنصر الخطة وليس الدواء
                var success = await TreatmentService.DeleteTreatmentItemAsync(itemId, _accessToken);
                if (success)
                {
                    await LoadCurrentAddedItems();
                }
            }
            catch (Exception ex) { Console.WriteLine($"Delete Error: {ex.Message}"); }
        }

        private async Task SendFinalTreatmentPlan()
        {
            if (currentAddedItems == null || !currentAddedItems.Any()) return;

            try
            {
                isSubmitting = true;

                // استدعاء الإرسال النهائي (تأكد أن المسار في الخدمة {sessionId}/send-treatment-plan)
                var newMsg = await TreatmentService.SendTreatmentPlanToPatientAsync(Id, _accessToken);

                if (newMsg != null)
                {
                    messages.Add(newMsg); 
                    currentAddedItems.Clear(); 
                    _ = ScrollToBottom();
                }
            }
            catch (Exception ex) { Console.WriteLine($"Final Send Error: {ex.Message}"); }
            finally { isSubmitting = false; StateHasChanged(); }
        }
    }
    
}