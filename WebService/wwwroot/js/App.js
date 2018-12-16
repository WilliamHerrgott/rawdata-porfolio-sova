var viewModel = function() {
    var self = this;

    self.search_query = ko.observable('');
    
    self.posts = ko.observableArray([]);
    self.answers = ko.observableArray([]);
    self.comments = ko.observableArray([]);
    self.marks = ko.observableArray([]);
    
    
    self.postBody = ko.observable();
    self.postDate = ko.observable();
    self.postLinkMark = ko.observable();

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

    // For the cloud
    // just added some elements to the array so we can
    // get the binding to work
    self.words = ko.observableArray([
        { text: "A", weight: 13 },
        { text: "B", weight: 10.5 }]);

    /*self.getRelatedWords = function () {
        self.request('StackOverflow/search/words/' + self.search_query(), null, function (data, status) {
            self.words.removeAll();
            var newWords = [];
            $.each(data.items, function (i, item) {
                newWords.push(item);
            });
            ko.utils.arrayPushAll(self.words, newWords);
            self.words.valueHasMutated();
        }, 'GET', function () { });
    };*/

    self.getRelatedWords = function () {
        $.ajax({
            url: "https://localhost:5001/api/StackOverflow/search/words/" + self.search_query,
            type: "get",
            async: false,
            success: function (data) {
                self.words.removeAll();
                var newWords = [];
                $.each(data.items, function (i, item) {
                    newWords.push(item);
                });
                ko.utils.arrayPushAll(self.words, newWords);
                self.words.valueHasMutated();
                $("#wordcloud").jQCloud(self.words);
            },
            dataType: 'json',                
            error: function(jqXHR, status, error) { callback_error(jqXHR, status, error) },
            beforeSend: function(xhr, settings) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + self.loggedToken());
            }
        });
    }; 

    // History array
    self.history = ko.observableArray();

    self.tryRegister = function() {
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
        }, 'GET', function(){self.setAccountOFF()});
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
        self.posts([]);
        Cookies.remove('token');  
        Cookies.remove('login');
        self.loggedToken();
        self.loggedLogin();
        $('#loginForm').removeClass('d-none');
        $('#registerLink').removeClass('d-none');
        $('#loggedInMenu').addClass('d-none');
    };

    self.getHistory = function() {
        // self.history().destr
        self.request('history', null, function (data, status) {
            self.history.removeAll();
            var hist = [];
            $.each(data.items, function (i, item) {
                hist.push(item);
            });
            ko.utils.arrayPushAll(self.history, hist);
            self.history.valueHasMutated();
        }, 'GET', function (){});
    };
    
    self.search = function () {
        self.request('StackOverflow/search/best/' + self.search_query(), null, function (data, status) {
            self.posts.removeAll();
            var news = [];
            $.each(data.items, function (i, item) {
                news.push(item);
            });
            ko.utils.arrayPushAll(self.posts, news);
            self.posts.valueHasMutated();
        }, 'GET', function () {});
    };

    self.loadFullPost = function(data, event) {
        $('#markButton').removeClass('btn-success').attr("disabled", false);

        var dataUrl = event.target.getAttribute('data-url');
        
        self.request(getAPIUrl(dataUrl), null, function (data, status) {
            self.postBody(self.strip(data.body));
            self.postDate(data.creationDate);
            self.postLinkMark(getAPIUrl(data.clickHereToMark));
            
            self.request(getAPIUrl(data.answers), null, function (data1, status) {
                self.answers.removeAll();
                var news = [];
                console.log(data1);
                $.each(data1.items, function (i, item) {
                    var nbOfComment = 0;
                    self.request(getAPIUrl(item.comments), null, function(data2, success){
                        nbOfComment = data2.numberOfItems;
                    }, 'GET', function(){});
                    self.request(getAPIUrl(item.author), null, function(data2, success) {
                        news.push({
                            body: item.body,
                            nbComments: nbOfComment,
                            commentsUrl: item.comments,
                            date: item.creationDate,
                            author: data2.name
                        });
                    }, 'GET', function() {});
                });
                ko.utils.arrayPushAll(self.answers, news);
                self.answers.valueHasMutated();
            }, 'GET', function(){});
        }, 'GET', function(){})
    };
    
    self.updateComments = function(data, event){
        var dataUrl = event.target.closest('a').getAttribute('data-url');
        self.request(getAPIUrl(dataUrl), null, function (data, status) {
            self.comments.removeAll();
            var news = [];
            $.each(data.items, function (i, item) {
                self.request(getAPIUrl(item.author), null, function(data1, success) {
                    news.push({
                        body: item.body,
                        date: item.creationDate,
                        author: data1.name,
                        authorUrl: item.author
                    });
                }, 'GET', function(){});
            });
            ko.utils.arrayPushAll(self.comments, news);
            self.comments.valueHasMutated();
        }, 'GET', function(){})
    };
    
    self.markPost = function() {
        self.request(self.postLinkMark(), null, function(data, status){
            $('#markButton').addClass('btn-success').attr("disabled", true);
        }, 'POST', function(){$('#markButton').addClass('btn-success').attr("disabled", true);});
    };
    
    self.updateMarks = function(){
        self.request('marks', null, function (data, status) {
            self.marks.removeAll();
            var news = [];
            $.each(data.items, function (i, item) {
                self.request(getAPIUrl(item.post), null, function(data1, status){
                    news.push({body: data1.body, date: data1.creationDate, annotation: data1.annotation, url: item.post});
                }, 'GET', function(){});
            });
            ko.utils.arrayPushAll(self.marks, news);
            self.marks.valueHasMutated();
        }, 'GET', function(){})
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
    };
    
    self.strip = function (html) {
        var tmp = document.createElement("DIV");
        tmp.innerHTML = html;
        return tmp.textContent || tmp.innerText || "";
    }
};



function getAPIUrl(url){
    return url.substring(url.indexOf('api/')+4);
}

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
            xhttp.onreadystatechange = function () {
                if (this.readyState === 4) {
                    if (this.status === 200) { elmnt.innerHTML = this.responseText; }
                    if (this.status === 404) { elmnt.innerHTML = "Page not found."; }
                    /* Remove the attribute, and call this function once more: */
                    elmnt.removeAttribute("w3-include-html");
                    init();
                }
            };
            xhttp.open("GET", file, true);
            xhttp.send();
            /* Exit the function: */
            return;
        }
    }
    $.ajaxSetup({
        contentType: "application/json; charset=utf-8",
        async: false
    });

    var VM = new viewModel();

    //$('#wordcloud').jQCloud(VM.words);

    if (Cookies.get('token') != null && Cookies.get('login'))
        VM.setAccountON(Cookies.get('token'), Cookies.get('login'));

    ko.applyBindings(VM);
}


// Activates knockout.js
$(document).ready(function () {
    init();   
});
