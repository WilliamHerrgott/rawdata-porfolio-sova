var viewModel = function() {
    var self = this;

    self.search_query = ko.observable('');
    self.posts = ko.observableArray([]);


    self.loggedLogin = ko.observable('');
    self.loggedToken = ko.observable('');
    
    
    self.registerLogin = ko.observable('');
    self.registerPassword = ko.observable('');
    self.registerEmail = ko.observable('');
    self.registerLocation = ko.observable('');


    self.login = ko.observable('');
    self.password = ko.observable('');
    
    
    self.tryRegister = function() {
        var url = "https://localhost:5001/api/users/";
        $.post(url, ko.toJSON({Username:self.registerLogin, Password: self.registerPassword, Email: self.registerEmail, Location: self.registerLocation}),
            function() {
                self.loginToSOVA(self.registerLogin, self.registerPassword);
                
            }, "json");
    };

    
    
    self.tryLogin = function() {
        self.loginToSOVA(self.login, self.password);
    };

    self.loginToSOVA = function(login, password) {
        var url = "https://localhost:5001/api/users/login";
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


    self.tryLogout = function() {
        self.setAccountOFF();
    };
    
    self.setAccountOFF = function () {
        Cookies.remove('token');  
        Cookies.remove('login');
        $('#loginForm').removeClass('d-none');
        $('#registerLink').removeClass('d-none');
        $('#loggedInMenu').addClass('d-none');
    };
    
    
    
    
    self.search = function () {
        // $.getJSON("https://localhost:5001/api/, function (data) {
        //     // Now use this data to update your view models, 
        //     // and Knockout will update your UI automatically 
        //     self.posts.removeAll();
        //     $.each(data.items, function (i, item) {
        //         self.posts.push(item)
        //     })
        // });
        
        self.request('StackOverflow/search/' + self.search_query(), null, function (data, status) {
            self.posts.removeAll();
                $.each(data.items, function (i, item) {
                    self.posts.push(item)
                })
        });
    };
    
    
    self.request = function(path, dataJSON, callback) {
        $.ajax({
            url: "https://localhost:5001/api/" + path,
            dataType: 'json',
            data: dataJSON,
            success: function(data, status){callback(data, status)},
            beforeSend: function(xhr, settings) { xhr.setRequestHeader('Authorization','Bearer ' + self.loggedToken() ); }
        });
    }
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

