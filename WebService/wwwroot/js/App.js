var viewModel = function() {
    var self = this;

    self.search_query = ko.observable('');
    
    self.posts = ko.observableArray([]);
    self.markedPosts = ko.observableArray([]);

    // Infos stored in db
    self.loggedID = ko.observable('');
    self.loggedLogin = ko.observable('');
    self.loggedEmail = ko.observable('');
    self.loggedToken = ko.observable('');
    self.loggedLocation = ko.observable('');
    
    self.fullModifyPassword = ko.observable(false);
    
    // Modifying infos
    self.modifyLogin = ko.observable('');
    self.modifyLogin.focused = ko.observable('');
    self.modifyEmail = ko.observable('');
    self.modifyLocation = ko.observable('');
    self.modifyPassword = ko.observable('');
    self.modifyPasswordBis = ko.observable('');

    // Creating user
    self.registerLogin = ko.observable('');
    self.registerPassword = ko.observable('');
    self.registerEmail = ko.observable('');
    self.registerLocation = ko.observable('');

    // Login in
    self.login = ko.observable('');
    self.password = ko.observable('');

    // See if user is connected
    self.isConnected = ko.observable(false);
    
    
    self.tryRegister = function() {
        console.log("TE");
        var url = "https://localhost:5001/api/users/";
        $.post(url, ko.toJSON({Username:self.registerLogin, Password: self.registerPassword, Email: self.registerEmail, Location: self.registerLocation}),
            function() {
                self.loginToSOVA(self.registerLogin, self.registerPassword);
            }, "json")
            .fail(function() {
                $.alert({
                    title: 'Encountered an error!',
                    content: 'This user already exists',
                    type: 'red',
                    typeAnimated: true,
                    backgroundDismiss: true,
                    icon: 'fa fa-warning',
                    closeIcon: true,
                    closeIconClass: 'fa fa-close'
                });
            });
    };
        
    self.tryLogin = function() {
        self.loginToSOVA(self.login, self.password);
    };

    self.showFullModifyPassword = function (){
        self.fullModifyPassword(true);
    };
    
    self.modifyLogin.focused.subscribe(function(on) {
        if (!on && self.modifyLogin() !== self.loggedLogin()) {
            self.request("users/update/username/" + self.modifyLogin, null, function(data, status) {
            }, 'PUT', function(){});
        }
    });
    
    self.cancelModifyPassword = function() {
    };
    
    self.modifyPassword = function() {
    };

    self.loginToSOVA = function(login, password) {
        var url = "https://localhost:5001/api/users/login";
        $.post(url, ko.toJSON({Username:login, Password: password}),
            function(data, textStatus) {
                Cookies.set('token', data.token, { expires: 7 });
                Cookies.set('login', data.username, { expires: 7 });
                self.setAccountON(data.token, data.username);
            }, "json")
            .fail(function() {
                $.alert({
                    title: 'Encountered an error!',
                    content: 'Bad login or password',
                    type: 'red',
                    typeAnimated: true,
                    backgroundDismiss: true,
                    icon: 'fa fa-warning',
                    closeIcon: true,
                    closeIconClass: 'fa fa-close'
                });
            });
    };

    self.setAccountON = function (token, login) {
        self.isConnected(true);
        self.loggedToken(token);
        self.loggedLogin(login);
        self.modifyLogin(login);
        self.request("users", null, function(data, status) {
            self.loggedLocation(data.location);
            self.modifyLocation(data.location);
            self.loggedEmail(data.email);
            self.modifyEmail(data.email);
            self.loggedID(data.id);
        }, 'GET', function(){});
        $('#registerModal').modal('hide');
        $('#loginForm').addClass('d-none');
        $('#registerLink').addClass('d-none');
        $('#loggedInMenu').removeClass('d-none');
    };


    self.tryLogout = function() {
        self.setAccountOFF();
    };
    
    self.setAccountOFF = function () {
        self.isConnected(false);
        Cookies.remove('token');  
        Cookies.remove('login');
        self.loggedToken();
        self.loggedLogin();
        $('#loginForm').removeClass('d-none');
        $('#registerLink').removeClass('d-none');
        $('#loggedInMenu').addClass('d-none');
    };
    
    self.search = function () {
        self.request('StackOverflow/search/best/' + self.search_query(), null, function (data, status) {
            self.posts.removeAll();
            var news = [];
                $.each(data.items, function (i, item) {
                    // self.posts.push(item)
                    news.push(item);
                });
            ko.utils.arrayPushAll(self.posts, news);
            self.posts.valueHasMutated();
        }, 'GET', function () {});
    };
    
    self.request = function(path, dataJSON, callback, type, callback_error) {
        $.ajax({
            url: "https://localhost:5001/api/" + path,
            type: type,
            dataType: 'json',
            data: dataJSON,
            success: function(data, status){callback(data, status)},
            error: function(jqXHR, status, error){callback_error(jqXHR, status, error)},
            beforeSend: function(xhr, settings) { xhr.setRequestHeader('Authorization','Bearer ' + self.loggedToken() ); }
        });
    }
};

function init() {
    var z, i, elmnt, file, xhttp;
    /* Loop through a collection of all HTML elements: */
    z = document.getElementsByTagName("*");
    for (i = 0; i < z.length; i++) {
        elmnt = z[i];
        /*search for elements with a certain atrribute:*/
        file = elmnt.getAttribute("w3-include-html");
        if (file) {
            /* Make an HTTP request using the attribute value as the file name: */
            xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function() {
                if (this.readyState === 4) {
                    if (this.status === 200) {elmnt.innerHTML = this.responseText;}
                    if (this.status === 404) {elmnt.innerHTML = "Page not found.";}
                    /* Remove the attribute, and call this function once more: */
                    elmnt.removeAttribute("w3-include-html");
                    init();
                }
            };
            xhttp.open("GET", file, true);
            xhttp.send();
            /* Exit the function: */
            console.log("try : " +i);
            return;
        }
    }
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
    console.log('end of binding');
}


// Activates knockout.js
$(document).ready(function() {
    init();
});

