
require.config({
    baseUrl: "js",
    paths: {
        jquery: "lib/jQuery/dist/jquery.min",
        knockout: "lib/knockout/dist/knockout.debug",
        dataService: 'services/dataService',
        bootstrap: 'lib/bootstrap.min',
        jqcloud: 'lib/jqcloud2/dist/jqcloud',
    },
    shim: {
        // set default deps
        'jqcloud': ['jquery']
    }
});

/*require(['knockout', 'app/posts'], function (ko, postVm) {
    ko.applyBindings(postVm);
});*/
