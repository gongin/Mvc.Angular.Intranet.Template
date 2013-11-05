var app = angular.module('app', [
  'titleService',
  'ui.router',
  'ui.bootstrap',
  'underscore',
  'stringjs',
  'home'
]);

// Having the inputs prefixed before the method protects them from minification, otherwise the injection could fail.
// Only show the default and routing for this page.  Each "module" defines it's own config and they are put together.  Allows for easy transfer of parts.
app.config(['$stateProvider', '$urlRouterProvider', '$locationProvider', function ($stateProvider, $urlRouterProvider, $locationProvider) {
  $locationProvider.html5Mode(true).hashPrefix('!');

  $urlRouterProvider.otherwise('/home');

  /*
  // Example of page navigation based on url parameters: If the incoming location has a query property, redirect to the proper search page
  $urlRouterProvider.rule(function ($injector, $location) {
    if ($location.search().query !== undefined) {
      return '/search';
    }
  });
  */
}]);

// Example of a service that maintains the title bar for each page.  Code is in Common/services and written in CoffeeScript!
app.run(['titleService', function (titleService) {
  titleService.setSuffix(' | Mvc-Ng-Template');
}])

app.controller('appCtrl', ['$scope', function ($scope) {

}]);