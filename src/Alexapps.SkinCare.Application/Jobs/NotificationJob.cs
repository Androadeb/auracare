using Alexapps.SkinCare.Entities.Users;
using Alexapps.SkinCare.Notifications;
using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Alexapps.SkinCare.Jobs
{
    public class NotificationJob : AsyncBackgroundJob<SendNotificationArgs>, ITransientDependency
    {
        private readonly IRepository<UserDevice, Guid> _userDeviceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public NotificationJob(
            IRepository<UserDevice, Guid> userDeviceRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _userDeviceRepository = userDeviceRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        // Use UnitOfWork to ensure DB connection is open during background execution
        public override async Task ExecuteAsync(SendNotificationArgs args)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var devices = await _userDeviceRepository.GetListAsync(
                    x => x.UserId == args.UserId && x.IsActive
                );

                if (devices == null || !devices.Any())
                {
                    return;
                }

                var tokens = devices.Select(x => x.Token).ToList();

                var message = new MulticastMessage()
                {
                    Tokens = tokens,
                    Notification = new Notification()
                    {
                        Title = args.Title,
                        Body = args.Body
                    },
                    Data = args.Data,
                    Android = GetAndroidConfig(),
                    Apns = GetIosConfig(),
                    Webpush = GetWebConfig()
                };

                try
                {
                    var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

                    if (response.FailureCount > 0)
                    {
                        var failedTokens = new List<string>();
                        for (var i = 0; i < response.Responses.Count; i++)
                        {
                            if (!response.Responses[i].IsSuccess)
                            {
                                var errorCode = response.Responses[i].Exception.MessagingErrorCode;
                                if (errorCode == MessagingErrorCode.Unregistered ||
                                    errorCode == MessagingErrorCode.InvalidArgument)
                                {
                                    failedTokens.Add(tokens[i]);
                                }
                            }
                        }

                        if (failedTokens.Any())
                        {
                            // Delete failed tokens to keep DB clean
                            await _userDeviceRepository.DeleteManyAsync(
                                devices.Where(d => failedTokens.Contains(d.Token))
                            );
                        }
                    }

                    await uow.CompleteAsync();
                }
                catch (Exception ex)
                {
                    // Log error (consider using ILogger)
                    Console.WriteLine($"Firebase Job Error: {ex.Message}");
                }
            }
        }

        private AndroidConfig GetAndroidConfig() => new AndroidConfig
        {
            Priority = Priority.High,
            Notification = new AndroidNotification { Sound = "default", ClickAction = "TOP_LEVEL_SCENE" }
        };

        private ApnsConfig GetIosConfig() => new ApnsConfig
        {
            Aps = new Aps { Sound = "default", Badge = 1 }
        };

        private WebpushConfig GetWebConfig() => new WebpushConfig
        {
            FcmOptions = new WebpushFcmOptions { Link = "https://skincare.alexapps.com/notifications" }
        };
    }
}
