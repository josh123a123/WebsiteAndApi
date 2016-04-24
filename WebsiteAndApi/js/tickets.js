function StudentTicketViewModel() {
	var Self = this;
	Self.Email = ko.observable();
	Self.StatusMessage = ko.observable();

	Self.GetCode = function () {
		var CodeRequest = new XMLHttpRequest();
		CodeRequest.open('POST', '/api/v1/ticket', true);
		CodeRequest.setRequestHeader('Content-Type', 'application/json');
		var str = '{"id":0,"email":"' + Self.Email() + '","code":null}';
		CodeRequest.send( str );

		CodeRequest.onreadystatechange = function () {
			if (CodeRequest.readyState == CodeRequest.DONE) {
				switch (CodeRequest.status) {
					case 201:
						Self.StatusMessage( 'You email address has been verified. The code has been sent.' );
						break;

					case 400:
						Self.StatusMessage('There was an error with the request.');
						break;

					case 500:
					case 502:
						Self.StatusMessage('There was a server side error. If this error persist, please contant info@devspaceconf.com');

					default:
						break;
				}
			}
		};
	};
}

ko.applyBindings(new StudentTicketViewModel(), document.getElementById('Content'));
