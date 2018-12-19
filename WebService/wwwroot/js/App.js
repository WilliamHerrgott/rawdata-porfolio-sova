var viewModel = function() {
    var self = this;

    self.search_query = ko.observable('');
    
    self.posts = ko.observableArray([]);
    self.answers = ko.observableArray([]);
    self.comments = ko.observableArray([]);
    self.marks = ko.observableArray([]);
    
    // Next and prev
    self.nextAnswers = ko.observable(null);
    self.prevAnswers = ko.observable(null);
    
    self.nextMarks = ko.observable(null);
    self.prevMarks = ko.observable(null);
    
    self.nextComments = ko.observable(null);
    self.prevComments = ko.observable(null);

    self.nextPosts = ko.observable(null);
    self.prevPosts = ko.observable(null);
    
    self.nextHistory = ko.observable(null);
    self.prevHistory = ko.observable(null);
    
    // Posts informations
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
    self.modifiedLogin = ko.observable('');
    self.modifiedEmail = ko.observable('');
    self.modifiedLocation = ko.observable('');
    self.modifiedPassword = ko.observable('');
    self.modifiedPasswordBis = ko.observable('');

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

    // History array
    self.history = ko.observableArray();
    
    // Word cloud array
    self.wordCloud = ko.observableArray();

    // Generic alert box function
    self.alertBox = function(title, content, type) {
        $.alert({
            title: title,
            content: content,
            type: type,
            typeAnimated: true,
            backgroundDismiss: true,
            closeIcon: true,
            closeIconClass: 'fa fa-close'
        });
    };
    
    self.tryRegister = function() {
        var url = "https://localhost:5001/api/users/";
        $.post(url, ko.toJSON({Username:self.registerLogin, Password: self.registerPassword, Email: self.registerEmail, Location: self.registerLocation}),
            function() {
                self.loginToSOVA(self.registerLogin, self.registerPassword);
            }, "json")
            .done(function() {
                self.registerLogin('');
                self.registerLocation('');
                self.registerEmail('');
                self.registerPassword('');
            })
            .fail(function() {
                self.alertBox('Encountered an error!', 'This user already exists', 'red');
            });
    };
        
    self.tryLogin = function() {
        self.loginToSOVA(self.login, self.password);
    };

    self.showFullModifyPassword = function (){
        self.fullModifyPassword(true);
    };
    
    self.changeInformations = function() {
        var loginChanged = false;
        var locationChanged = false;
        var emailChanged = false;
        
        if (self.loggedLogin() !== self.modifiedLogin()) {
            self.request("users/update/username/" + self.modifiedLogin(), null, function(data, status) {
            }, 'PUT', function(){});
            self.loggedLogin(self.modifiedLogin());
            loginChanged = true;
        }
        if (self.loggedLocation() !== self.modifiedLocation()) {
            self.request("users/update/location/" + self.modifiedLocation(), null, function(data, status) {
            }, 'PUT', function(){});
            self.loggedLocation(self.modifiedLocation());
            locationChanged = true;
        }
        if (self.loggedEmail() !== self.modifiedEmail()) {
            self.request("users/update/email/" + self.modifiedEmail(), null, function(data, status) {
            }, 'PUT', function(){});
            self.loggedEmail(self.modifiedEmail());
            emailChanged = true;
        }
        if (loginChanged !== false || locationChanged !== false || emailChanged !== false) {
            self.alertBox('Changes applied', 'Changes applied', 'green');
        }
    };
    
    self.resetCommonInfos = function() {
        self.modifiedEmail(self.loggedEmail());
        self.modifiedLogin(self.loggedLogin());
        self.modifiedLocation(self.loggedLocation());
    };
    
    self.resetPasswordInfos = function() {
        self.modifiedPassword(self.password());
        self.modifiedPasswordBis('');
    };
    
    self.modifyPassword = function() {
        if (self.modifiedPassword() === self.modifiedPasswordBis()) {
            if (self.modifiedPassword() !== self.password()) {
                self.request("users/update/password", ko.toJSON({Password: self.modifiedPassword()}), function(){
                }, 'PUT', function(){});
                self.password(self.modifiedPassword());
                self.resetPasswordInfos();
                self.alertBox('Changes applied', 'Password changed', 'green');
            } else {
                self.resetPasswordInfos();
                self.alertBox('Encountered an error!', 'New password and old password are the same', 'red');
            }
        } else {
            self.resetPasswordInfos();
            self.alertBox('Encountered an error!', 'The confirmation does not match', 'red');
        }
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
                self.alertBox('Encountered an error!', 'Bad login or password', 'red');
            });
    };

    self.setAccountON = function (token, login) {
        self.isConnected(true);
        self.loggedToken(token);
        self.loggedLogin(login);
        self.modifiedLogin(login);
        self.request("users", null, function(data, status) {
            self.loggedLocation(data.location);
            self.modifiedLocation(data.location);
            self.loggedEmail(data.email);
            self.modifiedEmail(data.email);
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

    self.updateHistory = function() {
        self.loadHistory('history');
    };
    
    self.loadHistory = function(url) {
        self.request(getAPIUrl(url), null, function (data, status) {
            self.history.removeAll();
            var hist = [];
            $.each(data.items, function (i, item) {
                hist.push(item);
            });
            self.nextHistory(data.next);
            self.prevHistory(data.prev);

            ko.utils.arrayPushAll(self.history, hist);
            self.history.valueHasMutated();
        }, 'GET', function (){});
    };
    
    self.reLoadHistory = function(data, event) {
        var dataUrl = event.target.getAttribute('data-url');
        self.loadHistory(dataUrl);
    };
    
    self.search = function () {
        self.updatePosts('StackOverflow/search/best/' + self.search_query())
    };
    
    self.updatePosts = function(url) {
        self.request(getAPIUrl(url), null, function (data, status) {
            $('#wordcloud').jQCloud('destroy');
            self.posts.removeAll();
            var news = [];
            $.each(data.items, function (i, item) {
                news.push(item);
            });
            self.nextPosts(data.next);
            self.prevPosts(data.prev);
            ko.utils.arrayPushAll(self.posts, news);
            self.posts.valueHasMutated();
        }, 'GET', function () {});
    };

    self.reLoadPosts = function(data, event){
        var dataUrl = event.target.getAttribute('data-url');
        self.updatePosts(dataUrl);
    };

    self.getRelatedWords = function () {
        self.request('StackOverflow/search/words/' + self.search_query(), null, function (data, status) {
            self.posts.removeAll();
            self.prevPosts(null);
            self.nextPosts(null);
            self.wordCloud.removeAll();
            var newWords = [];
            $.each(data.items, function (i, item) {
                newWords.push(item);
            });
            ko.utils.arrayPushAll(self.wordCloud, newWords);
            self.wordCloud.valueHasMutated();
        }, 'GET', function (){});
    };
    
    self.isPostMarked = function() {
        var isMarked;

        self.request(self.postLinkMark(), null, function(data, status) {
            isMarked = data;
        }, 'GET', function (){});

        if (isMarked === true) {
            $('#markButton').addClass('btn-success').attr("disabled", true);
        } else {
            $('#markButton').removeClass('btn-success').attr("disabled", false);
        }
    };
    
    self.loadFullPost = function(data, event) {
        var dataUrl = event.target.getAttribute('data-url');
        
        self.request(getAPIUrl(dataUrl), null, function (data, status) {
            self.postBody(self.strip(data.body));
            self.postDate(data.creationDate);
            self.postLinkMark(getAPIUrl(data.clickHereToMark));
            
            self.updateAnswers(data.answers);
            
        }, 'GET', function(){});
        
        // Mark the post
        self.isPostMarked();
    };
    
    self.updateAnswers = function(url) {
        self.request(getAPIUrl(url), null, function (data, status) {
            self.answers.removeAll();
            var news = [];
            $.each(data.items, function (i, item) {
                var nbOfComment = 0;
                self.request(getAPIUrl(item.comments), null, function(data1, success){
                    nbOfComment = data1.numberOfItems;
                }, 'GET', function(){});
                self.request(getAPIUrl(item.author), null, function(data1, success) {
                    news.push({
                        body: item.body,
                        nbComments: nbOfComment,
                        commentsUrl: item.comments,
                        date: item.creationDate,
                        author: data1.name
                    });
                }, 'GET', function() {});
            });
            self.nextAnswers(data.next);
            self.prevAnswers(data.prev);
            ko.utils.arrayPushAll(self.answers, news);
            self.answers.valueHasMutated();
        }, 'GET', function(){});
    };
    
    self.reLoadAnswers = function(data, event) {
        var dataUrl = event.target.getAttribute('data-url');
        self.updateAnswers(dataUrl);
    };
    
    self.updateComments = function(data, event) {
        var dataUrl = event.target.closest('[data-url]').getAttribute('data-url');
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
            self.nextComments(data.next);
            self.prevComments(data.prev);

            ko.utils.arrayPushAll(self.comments, news);
            self.comments.valueHasMutated();
        }, 'GET', function(){})
    };
    
    self.markPost = function() {
        self.request(self.postLinkMark(), null, function(data, status){
            $('#markButton').addClass('btn-success').attr("disabled", true);
        }, 'POST', function(){$('#markButton').addClass('btn-success').attr("disabled", true);});
    };
    
    self.updateMarks = function() {
        self.loadMarks('marks')
    };
    
    self.loadMarks = function(url) {
        self.request(getAPIUrl(url), null, function (data, status) {
            self.marks.removeAll();
            var news = [];
            $.each(data.items, function (i, item) {
                self.request(getAPIUrl(item.post), null, function(data1, status){
                    news.push({body: data1.body, date: data1.creationDate, annotation: data1.annotation, url: item.post});
                }, 'GET', function(){});
            });
            self.nextMarks(data.next);
            self.prevMarks(data.prev);

            ko.utils.arrayPushAll(self.marks, news);
            self.marks.valueHasMutated();
        }, 'GET', function(){})
    };

    self.reLoadMarks = function(data, event){
        var dataUrl = event.target.getAttribute('data-url');
        self.loadMarks(dataUrl);
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
    if (url.indexOf('api/') === -1) {
        return url;
    }
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
    
    if (Cookies.get('token') != null && Cookies.get('login'))
        VM.setAccountON(Cookies.get('token'), Cookies.get('login'));

    ko.applyBindings(VM);
    
    // Wordcloud with jqcloud
    VM.wordCloud.subscribe(function () {
        $('#wordcloud').jQCloud('update', VM.wordCloud(), {
            width: 500,
            height: 350
        });
    });
}

// Activates knockout.js
$(document).ready(function () {
    init();   
});
