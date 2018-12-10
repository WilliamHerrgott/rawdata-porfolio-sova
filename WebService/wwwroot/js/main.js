
require.config({
    baseUrl: "js",
    paths: {
        jquery: "lib/jQuery/dist/jquery.min",
        knockout: "lib/knockout/dist/knockout.debug",
        dataService: 'services/dataService',
        bootstrap: 'lib/bootstrap.min'
    }
});

require(['knockout', 'app/posts'], function (ko, postVm) {
    ko.applyBindings(postVm);
});
