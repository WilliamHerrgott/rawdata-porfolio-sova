var viewModel = function() {
    var self = this;

    self.search_query = ko.observable('');
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
    var VM = new viewModel();


    VM.search_query.subscribe(function () {
        VM.search();
    });
    
    ko.applyBindings(VM);
});

