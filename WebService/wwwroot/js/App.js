var viewModel = function() {
    var self = this;

    self.search_query = ko.observable('');
    self.posts = ko.observableArray([
        { title: 'Bert', score: 'Bertington' , body: "test1"},
        { title: 'Bert1', score: 'Bertington1' , body: "test2"},
        { title: 'Bert2', score: 'Bertington2' , body: "test3"},
        { title: 'Bert3', score: 'Bertington3' , body: "test4"},
        { title: 'Bert4', score: 'Bertington4' , body: "test5"}
    ]);

    self.search = function () {
        $.getJSON("http://localhost:5000/api/StackOverflow/search/"+self.search_query(), function(data) {
            // Now use this data to update your view models, 
            // and Knockout will update your UI automatically 
            console.log(data)
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

