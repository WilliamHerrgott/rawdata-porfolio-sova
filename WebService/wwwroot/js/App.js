var viewModel = function() {
    var self = this;

    self.search_query = ko.observable('');
    
    self.loggedLogin = ko.observable('');
    self.loggedToken = ko.observable('');
    
    self.registerLogin = ko.observable('');
    self.registerPassword = ko.observable('');
    self.registerEmail = ko.observable('');
    self.registerLocation = ko.observable('');
    
    
    self.tryRegister = function() {
        var url = "http://localhost:5000/api/users/";
        $.post(url, ko.toJSON({Username:self.registerLogin, Password: self.registerPassword, Email: self.registerEmail, Location: self.registerLocation}),
            function() {
                self.loginToSOVA(self.registerLogin, self.registerPassword);
                
            }, "json");
    };


    self.loginToSOVA = function(login, password) {
        var url = "http://localhost:5000/api/users/login";
        $.post(url, ko.toJSON({Username:login, Password: password}), function(data, textStatus) {
            Cookies.set('token', data.token, { expires: 7 });
            Cookies.set('login', data.username, { expires: 7 });
            self.setAccountON(data.token, data.username);
        }, "json");
    };


    self.setAccountON = function (token, login) {
        self.loggedToken(token);
        self.loggedLogin(login);
        $('#registerModal').modal('hide');
        $('#loginForm').addClass('d-none');
        $('#registerLink').addClass('d-none');
        $('#loggedInMenu').removeClass('d-none');
    };
    
    
    self.login = ko.observable('');
    self.password = ko.observable('');
    self.tryLogin = function() {
        self.loginToSOVA(self.login, self.password);
    };
    
    self.posts = ko.observableArray([]);

    self.search = function () {
        $.getJSON("http://localhost:5000/api/StackOverflow/search/"+self.search_query(), function(data) {
            // Now use this data to update your view models, 
            // and Knockout will update your UI automatically 
            self.posts.removeAll();
            $.each(data.items, function (i, item) {
                self.posts.push(item)
            })
        });
    };
};

// Activates knockout.js
$(document).ready(function() {
    $.ajaxSetup({
        contentType: "application/json; charset=utf-8"
    });
    
    var VM = new viewModel();
    
    
    VM.search_query.subscribe(function () {
        VM.search();
    });

    if (Cookies.get('token') != null && Cookies.get('login'))
        VM.setAccountON(Cookies.get('token'), Cookies.get('login'));
    
    ko.applyBindings(VM);
});

