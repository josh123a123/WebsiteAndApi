// You're lucky I have too much respect for bandwidth to reproduce
// the profanity filled rant that accompanied the following functions...
function GetDay(date) {
	switch (date.getDay()) {
		case 0:
			return 'Sunday';
		case 1:
			return 'Monday';
		case 2:
			return 'Tuesday';
		case 3:
			return 'Wednesday';
		case 4:
			return 'Thursday';
		case 5:
			return 'Friday';
		case 6:
			return 'Saturday';
	}
}

function GetMonth(date) {
	switch (date.getMonth()) {
		case 1:
			return 'January';
		case 2:
			return 'February';
		case 3:
			return 'March';
		case 4:
			return 'April';
		case 5:
			return 'May';
		case 6:
			return 'June';
		case 7:
			return 'July';
		case 8:
			return 'August';
		case 9:
			return 'September';
		case 10:
			return 'October';
		case 11:
			return 'November';
		case 12:
			return 'December';
	}
}

function FormatDate(date) {
	var AmPm = 'AM';
	var hours = date.getHours();
	if (hours > 12) {
		AmPm = 'Pm';
		hours -= 12;
	}

	var minutes = date.getMinutes();
	if (minutes < 10) {
		minutes = '0' + new String(date.getMinutes());
	}

	return GetDay(date) + ', ' + GetMonth(date) + ' ' + date.getDate() + ' at ' + hours + ':' + minutes + ' ' + AmPm;
}

function Profile(data) {
	var Self = this;
	Self.Id = ko.observable();
	Self.DisplayName = ko.observable();
	Self.Link = ko.observable();

	if (data) {
		Self.Id(data.Id);
		Self.DisplayName(data.DisplayName);

		Self.Link('/speakers.html?id=' + data.Id);
	}
}

function TimeSlot(data) {
	var Self = this;
	Self.StartTime = ko.observable();
	Self.EndTime = ko.observable();
	Self.DisplayDateTime = ko.pureComputed(function () {
		return FormatDate(new Date(this.StartTime()));
	}, Self);

	if (data) {
		Self.StartTime(data.StartTime);
		Self.EndTime(data.EndTime);
	}
}

function Session(data) {
	var Self = this;
	Self.Id = ko.observable();
	Self.Speaker = ko.observable();
	Self.Title = ko.observable();
	Self.Abstract = ko.observable();
	Self.TimeSlot = ko.observable();
	Self.Tags = ko.observableArray([]);

	Self.TagList = ko.pureComputed(function () {
		var TagList = '';
		for (var index = 0; index < this.Tags().length; ++index)
			TagList += this.Tags()[index].Text() + '; ';
		return TagList;
	}, Self);

	if (data) {
		Self.Id(data.Id);
		Self.Speaker(new Profile(data.Speaker));
		Self.Title(data.Title);
		Self.Abstract('<p>' + data.Abstract.trim().replace('\n', '</p><p>') + '</p>');
		Self.TimeSlot(new TimeSlot(data.TimeSlot));

		if (data.Tags)
			for (var index = 0; index < data.Tags.length; ++index)
				if (ko.isObservable(data.Tags[index]))
					Self.Tags.push(data.Tags[index]);
				else
					Self.Tags.push(new Tag(data.Tags[index]));
	}
}

function Tag(data) {
	var Self = this;
	Self.Id = ko.observable(data.Id);
	Self.Text = ko.observable(data.Text);
	Self.Link = ko.observable('sessions.html?tagId=' + data.Id);
}

function ViewModel() {
	var Self = this;
	Self.Sessions = ko.observableArray([]);

	var qd = null;
	if (location.search) {
		qd = {};
		location.search.substr(1).split("&").forEach(function (item) { var s = item.split("="), k = s[0], v = s[1] && decodeURIComponent(s[1]); (k in qd) ? qd[k].push(v) : qd[k] = [v] });
	}

	var SessionsRequest = new XMLHttpRequest();
	if (qd)
		if (qd.id)
			SessionsRequest.open('GET', '/api/v1/session/' + qd.id, true);
		else if (qd.tagId)
			SessionsRequest.open('GET', '/api/v1/session/tag/' + qd.tagId, true);
		else
			SessionsRequest.open('GET', '/api/v1/session', true);
	else
		SessionsRequest.open('GET', '/api/v1/session', true);
	SessionsRequest.send();

	SessionsRequest.onreadystatechange = function () {
		if (SessionsRequest.readyState == SessionsRequest.DONE) {
			switch (SessionsRequest.status) {
				case 200:
					var SessionList = JSON.parse(SessionsRequest.responseText);
					if (SessionList.length)
						for (var index = 0; index < SessionList.length; ++index)
							Self.Sessions.push(new Session(SessionList[index]));
					else
						Self.Sessions.push(new Session(SessionList));
					break;

				case 401:
					// Login failed

				default:
					break;
			}
		}
	};
}

ko.applyBindings(new ViewModel(), document.getElementById('Content'));
