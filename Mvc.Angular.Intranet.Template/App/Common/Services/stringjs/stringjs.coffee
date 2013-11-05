angular.module('stringjs', [])

.factory 'S', ['$window', ($window) ->
  return $window.S
]