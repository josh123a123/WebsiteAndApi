function SponsorLevel(data) {
	var Self = this;
	Self.Id = ko.observable();
	Self.DisplayOrder = ko.observable();
	Self.DisplayName = ko.observable();
	Self.DisplayInSidebar = ko.observable();
	Self.DisplayInEmail = ko.observable();

	if (data) {
		Self.Id(data.Id);
		Self.DisplayOrder(data.DisplayOrder);
		Self.DisplayName(data.DisplayName);
		Self.DisplayInSidebar(data.DisplayInSidebar);
		Self.DisplayInEmail(data.DisplayInEmail);
	}
}

function Sponsor(data) {
	var Self = this;
	Self.Id = ko.observable();
	Self.DisplayName = ko.observable();
	Self.Level = ko.observable();
	Self.LogoLarge = ko.observable();
	Self.LogoSmall = ko.observable();
	Self.Website = ko.observable();

	if (data) {
		Self.Id(data.Id);
		Self.DisplayName(data.DisplayName);
		Self.Level = new SponsorLevel(data.Level);
		Self.LogoLarge(data.LogoLarge);
		Self.LogoSmall(data.LogoSmall);
		Self.Website(data.Website);
	}
}

function SponsorSidebarViewModel() {
	var Self = this;
	Self.Sponsors = ko.observableArray([]);

	var SponsorRequest = new XMLHttpRequest();
	SponsorRequest.open('GET', '/api/v1/sponsor', true);
	SponsorRequest.send();

	SponsorRequest.onreadystatechange = function () {
		if (SponsorRequest.readyState == SponsorRequest.DONE) {
			switch (SponsorRequest.status) {
				case 200:
					var SponsorList = JSON.parse(SponsorRequest.responseText);
					if (SponsorList.length)
						for (var index = 0; index < SponsorList.length; ++index)
							Self.Sponsors.push(new Sponsor(SponsorList[index]));
					else
						Self.Sponsors.push(new Sponsor(SponsorList));
					break;

				case 401:
					// Login failed

				default:
					break;
			}
		}
	};
}

ko.applyBindings(new SponsorSidebarViewModel(), document.getElementById('RightSidebar'));

if (document.getElementById('MainSponsorElement')) ko.applyBindings(new SponsorSidebarViewModel(), document.getElementById('MainSponsorElement'));