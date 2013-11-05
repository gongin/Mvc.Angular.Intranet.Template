angular.module('underscore', [])

.factory '_', ['$window', ($window) ->
  underscore = $window._;

  # Extend to group by mutli-columns
  underscore.groupByMulti = (obj, values, context) ->
    if not values.length then return obj
    byFirst = underscore.groupBy obj, values[0], context
    rest = values.slice(1)      
    for prop in byFirst
      byFirst[prop] = underscore.groupByMulti byFirst[prop], rest, context      
    return byFirst;

  underscore.aggregate = (objects, nonAggregateProperties, aggregateProperties, aggregateFunction) ->
    if _.isArray(objects) and objects.length > 0 and _.has(objects[0], aggregateProperties[0])
      # Arrived
      # Union groupByProperties with aggregate Properties to create a template.
      # First hit are the values, remainders are adding to previous
      result = _.pick(objects[0], _.union(nonAggregateProperties, aggregateProperties))
      objects = objects.slice(1)
      result = underscore.reduceObject(objects, aggregateProperties, aggregateFunction, result)
      return result
    else
      # Keep looking
      result = [];
      for obj in objects
        intermediate = underscore.aggregate(objects[obj], nonAggregateProperties, aggregateProperties, aggregateFunction);
        result.push(intermediate);
      return result;

  underscore.groupByMultiWithAggregate = (items, groupByCols, aggregateProperties, aggregateFunction) ->
    workingData = _.groupByMulti(items, groupByCols)
    workingData = _.aggregate(workingData, groupByCols, aggregateProperties, aggregateFunction)
    if workingData.length isnt undefined 
        workingData = _.flatten(workingData);
    else
      # Case when no group by is presented and need single aggregate.
      workingData = [workingData];    
    return workingData
  return underscore
]