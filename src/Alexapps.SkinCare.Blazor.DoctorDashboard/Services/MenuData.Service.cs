using AlexApps.Classat.Blazor.Superadmin.Services;
using System.Collections.Generic;

namespace AlexApps.Classat.Blazor.Superadmin.Services
{
    public class MenuDataService
    {
        private bool _isArabic = true;
        private readonly JsonLocalizationService _loc;
        private List<MainMenuItems> _menuItems;

        public MenuDataService(JsonLocalizationService loc)
        {
            _loc = loc;
        }

        public void SetLanguage(bool isArabic)
        {
            _isArabic = isArabic;
            _menuItems = null;
        }

        public List<MainMenuItems> GetMenuData()
        {
            if (_menuItems == null)
            {
                _menuItems = new List<MainMenuItems>()
                {
                    // Dashboard
                    new MainMenuItems {
                        Path = "/admin/dashboard",
                        Type = "link",
                        Title = _loc["Dashboard"], // Ã·» „‰ «·‹ JSON
                        Icon = "ri-home-line",
                        Selected = false, Active = false
                    },

                    // Sessions
                    new MainMenuItems {
                        Path = "/admin/sessions",
                        Type = "link",
                        Title = _loc["Sessions"],
                        Icon = "ri-calendar-todo-line",
                        Selected = false, Active = false
                    },

                    // Medical Tests
                    new MainMenuItems {
                        Path = "/admin/medical-tests",
                        Type = "link",
                        Title = _loc["Medical Tests"],
                        Icon = "ri-file-list-3-line",
                        Selected = false, Active = false
                    },

                  
                    new MainMenuItems {
    Path = "/admin/scheduled-videos",
    Type = "link",
    Title = _loc["Scheduled Videos"],
    Icon = "ri-video-line", 
    Selected = false, Active = false
},

                  // Legal Information (Submenu)
new MainMenuItems {
    Type = "sub",
    Title = _loc["LegalInformation"], // Key: LegalInformation
    Icon = "ri-equalizer-line",
    Selected = false,
    Active = false,
    Children = new MainMenuItems[]
    {
        new MainMenuItems {
            Path = "/admin/legal/terms",
            Type = "link",
            Title = _loc["TermsConditions"], // Key: TermsConditions
            Selected = false,
            Active = false
        },
        new MainMenuItems {
            Path = "/admin/legal/privacy",
            Type = "link",
            Title = _loc["PrivacyPolicy"], // Key: PrivacyPolicy
            Selected = false,
            Active = false
        },
        new MainMenuItems {
            Path = "/admin/legal/faq",
            Type = "link",
            Title = _loc["FAQ"], // Key: FAQ
            Selected = false,
            Active = false
        }
    }
},

                    // Profile
                    new MainMenuItems {
                        Path = "/admin/profile",
                        Type = "link",
                        Title = _loc["Profile"],
                        Icon = "ri-user-settings-line",
                        Selected = false, Active = false
                    },

                    // Contact Us
                    new MainMenuItems {
                        Path = "/admin/contact-us",
                        Type = "link",
                        Title = _loc["Contact Us"],
                        Icon = "ri-phone-line",
                        Selected = false, Active = false
                    },

                    // Blog
                    new MainMenuItems {
                        Path = "/admin/blog",
                        Type = "link",
                        Title = _loc["Blog"],
                        Icon = "ri-article-line",
                        Selected = false, Active = false
                    },

                    // About
                    new MainMenuItems {
                        Path = "/admin/about",
                        Type = "link",
                        Title = _loc["About"],
                        Icon = "ri-information-line",
                        Selected = false, Active = false
                    },

                    // Logout
                   new MainMenuItems {
    Path = "/auth/logout",
    Type = "link",
    Title = _loc["Log out"],
    Icon = "ri-logout-box-r-line",
    Selected = false, Active = false
}
                };
            }
            return _menuItems;
        }
    }
}