/**
 * Each section of the site has its own module. It probably also has
 * submodules, though this template is too simple to demonstrate it. Within
 * `App/AppSource/home`, however, could exist several additional folders representing
 * additional modules that would then be listed as dependencies of this one.
 * For example, a `note` section could have the submodules `note.create`,
 * `note.delete`, `note.edit`, etc.
 *
 * Regardless, so long as dependencies are managed correctly, the build process
 * will automatically take take of the rest.
 *
 * The dependencies block here is also where component dependencies should be
 * specified, as shown below.
 */
angular.module('home', [
  'ui.router',
  'titleService',
  'placeholders'
])

/**
 * Each section or module of the site can also have its own routes. AngularJS
 * will handle ensuring they are all available at run-time, but splitting it
 * this way makes each module more "self-contained".
 */
.config(['$stateProvider', function ( $stateProvider ) {
  $stateProvider.state( 'home', {
    url: '/',
    views: {
      "": {
        controller: 'homeCtrl',
        templateUrl: '/HomeTpl/Index'
      }
    }
  });
}])

/**
 * And of course we define a controller for our route.
 */
.controller( 'homeCtrl', ['$scope', 'titleService', function ( $scope, titleService ) {
  titleService.setTitle('Home');
  $scope.imageDimension = '550x300';
}])

;

