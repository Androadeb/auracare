public class MenuDataService
{
	private List<MainMenuItems> MenuData = new List<MainMenuItems>()
	{
		new MainMenuItems(
			path: "/dashboard",
			type: "link",
			title: "Dashboard",
			icon: "../assets/images/menu/home.svg",
			selected: false,
			active: true,
			dirChange: false
		),
		new MainMenuItems(
			path: "/users",
			type: "link",
			title: "User",
			icon: "../assets/images/menu/users.svg",
			selected: false,
			active: false,
			dirChange: false
		),
		new MainMenuItems(
			path: "/medical-tests",
			type: "link",
			title: "Medical Tests",
			icon: "../assets/images/menu/report-medical.svg",
			selected: false,
			active: false,
			dirChange: false
		),
		new MainMenuItems(
			path: "/orders",
			type: "link",
			title: "Orders",
			icon: "../assets/images/menu/orders.svg",
			selected: false,
			active: false,
			dirChange: false
		),
		new MainMenuItems(
			title: "Legal Information",
			type: "sub",
			icon: "../assets/images/menu/legal.png",
			selected: false,
			active: false,
			dirChange: false,
			children: new MainMenuItems[]
			{
				new MainMenuItems(
					path: "/legal/terms-and-conditions",
					title: "Terms & Conditions",
					type: "link",
					selected: false,
					active: false,
					dirChange: false
				),
				new MainMenuItems(
					path: "/legal/privacy-policy",
					title: "Privacy Policy",
					type: "link",
					selected: false,
					active: false,
					dirChange: false
				),
				new MainMenuItems(
					path: "/legal/frequently-questions",
					title: "Frequently Asked Questions",
					type: "link",
					selected: false,
					active: false,
					dirChange: false
				)
			}
		),
		new MainMenuItems(
			path: "/contact-us",
			type: "link",
			title: "Contact us",
			icon: "../assets/images/menu/contact.svg",
			selected: false,
			active: false,
			dirChange: false
		),
		new MainMenuItems(
			path: "/financial",
			type: "link",
			title: "Financial",
			icon: "../assets/images/menu/money.svg",
			selected: false,
			active: false,
			dirChange: false
		),
		new MainMenuItems(
			path: "/profile",
			type: "link",
			title: "Profile",
			icon: "../assets/images/menu/user-circle.svg",
			selected: false,
			active: false,
			dirChange: false
		),
		//new MainMenuItems(
		//		path: "/blog",
		//		type: "link",
		//		title: "Blog",
		//		icon: "../assets/images/menu/blog.svg",
		//		selected: false,
		//		active: false,
		//		dirChange: false
		//),
		new MainMenuItems(
			path: "/about",
			type: "link",
			title: "About",
			icon: "../assets/images/menu/info-circle.svg",
			selected: false,
			active: false,
			dirChange: false
		),
		new MainMenuItems(
			path: "/logout",
			type: "button",
			title: "Log out",
			icon: "../assets/images/menu/logout.svg",
			selected: false,
			active: false,
			dirChange: false
		)
	};

	public List<MainMenuItems> GetMenuData()
	{
		return MenuData;
	}
}
