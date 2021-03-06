What Not To Do
-----------

Auto-mappers are great, but you should not assume everything you add will be mapped automatically and successfully since most of them has default behaviour of ignoring non-mappable properties. How do you catch mistakes like:

1. You added new property on domain model and view model, but spelled them differently so auto-mapper did not recognize them to be mapped.
2. You added new property on domain model and repository's Get() function, but did not change Update() function.

Also, you don't want to have test code that repeats mapper code. For example:

```c#
[Test]
public void Mapper_test()
{
    var domainModel = new DomainModel();
	domainModel.Value1 = "value 1";
	domainModel.Value2 = "value 2";
	
	var viewModel = new ViewModelFactory().BuildFrom(domainModel);
    
	Assert.That(viewModel.Value1, Is.EqualTo("value 1"));
	Assert.That(viewModel.Value2, Is.EqualTo("value 2"));
}
```

If ViewModelFactory.BuildFrom() assigns values one by one, this test repeats implementation of ViewModelFactory.BuildFrom(). If ViewModelFactory.BuildFrom() uses auto-mappers, this test does not catch newly added properties in case they are not mapped.

Using ObjectTestHelpers, you can test the mappers better, either you hand-wrote it or used auto-mapper.

Testing Bi-Directional Mapping
-----------

Repository pattern can be seen as bi-directional mapping. It maps your domain object to database layer, and vice versa. 

So how can we test repository code? The idea is,

1. Create a domain object and fill in with values
2. Save
3. Retrieve from database
4. Make sure the retrieved one is the same as the first one you asked the repository to save.

Using ObjectTestHelpers, the code looks like this:

```c#
[Test]
public void Restored_object_must_match_with_original_object()
{
    var repository = new SampleRepository();
	
    var domainModel1 = BuildSampleDomainModel();
    var id = repository.Save(domainModel1);
    var domainModel2 = repository.GetById(id);

    new ObjectComparer().AssertEquality(domainModel1, domainModel2, new[]
    {
        "/CreatedAt - "     // CreatedAt is supposed to be different.
    });
}
```

Using SampleValueSetter class in BuildSampleDomainModel(), you don't touch this test ever when you add new properties to domain model.

```c#
private SampleDomainModel BuildSampleDomainModel()
{
    var model = new SampleDomainModel();
    var setter = new SampleValueSetter();
    setter.AssignSampleValues(model, 0);
    return model;
}
```

AssignSampleValues() takes a seed value, and assign 0, 1, 2, ... to properties (and fields) on model object. It will travel deep when properties are class object, list or dictionary.

Testing One-Way Mapping
-----------

Sometimes mapping is one way. You map from domain model to view model, but changes are not likely be recorded using the same view model. 

How can we test one-way mapping? The idea is that source property change should result in one of the destination property to be changed.

1. Create source object #1 and fill in with values, using seed value 0.
2. Create source object #2 and fill in with values, using seed value 1. This means every property value in source object #1 is different from corresponding property value in source object #2.
3. Map both objects and get two destination objects.
4. Compare each properties in the destination object #1 and #2, and assert they are all different.

```c#
public void Source_property_change_should_result_in_destination_property_change()
{
    var setter = new SampleValueSetter();
    var viewModelFactory = new SampleViewModelFactory();

    var domainModel1 = new SampleDomainModel();
    setter.AssignSampleValues(domainModel1, 0);
    var viewModel1 = viewModelFactory.Build(domainModel1);

    var domainModel2 = new SampleDomainModel();
    setter.AssignSampleValues(domainModel2, 1);
    var viewModel2 = viewModelFactory.Build(domainModel2);

    new ObjectComparer().AssertDifference(viewModel1, viewModel2, new []
    {
        "/StaticValue - static value"		// ViewModel.StaticValue is not mapped from DomainModel.
    });
}
```

What Happens When There is a Problem
-------------
I added IntValue2 on the SampleDomainModel and did not revise SampleRepository. Now BidirectionalMappingTest fails with the following error message. Note that it detected IntValue2 as Changed property, which is a problem.

```
ObjectTestHelpers.AssertDifferenceException : 1 issue(s) found. See output.
......
PROBLEM: Changed property detected: /IntValue2 - 6

Object 1: 
/IntValue - 1
/StringValue - 2
/DateTimeValue - 1/4/2012 12:00:00 AM
/GuidValue - 00000000-0000-0000-0000-000000000004
/BoolValue - True
/IntValue2 - 6
/CreatedAt - 1/8/2012 12:00:00 AM

Object 2: 
/IntValue - 1
/StringValue - 2
/DateTimeValue - 1/4/2012 12:00:00 AM
/GuidValue - 00000000-0000-0000-0000-000000000004
/BoolValue - True
/IntValue2 - 0
/CreatedAt - 7/29/2015 2:29:53 PM
```

Now I added IntValueString2 on the SampleViewModel and did not revise SampleViewModelFactory. Now OneWayMappingTest fails with the following error message. Note that it detected IntValueString2 as Unchanged property, which is a problem.

```
ObjectTestHelpers.AssertDifferenceException : 1 issue(s) found. See output.
......
PROBLEM: Unchanged property detected: /IntValueString2 - null

Object 1: 
/IntValueString - 1
/StringValue - 2
/DateTimeValueString - 1/4/2012 12:00:00 AM
/GuidValueString - 00000000-0000-0000-0000-000000000004
/BoolValueString - True
/IntValueString2 - null
/StaticValue - static value

Object 2: 
/IntValueString - 2
/StringValue - 3
/DateTimeValueString - 1/5/2012 12:00:00 AM
/GuidValueString - 00000000-0000-0000-0000-000000000005
/BoolValueString - False
/IntValueString2 - null
/StaticValue - static value
```


Limitations
-------------
SampleValueSetter does not add an item automatically if an empty list or dictionary is found. As a result, you need to new-up an element and add it to the list before calling SampleValueSetter. Once it is added, SampleValueSetter will traverse deep to assign values.

One-way mapping testing method suggested here does not catch all problems, but it gets the job done reasonably well, especially if your mapping is one-to-one. 

Just a Thought: No Bi-Directional Mapping for View Model
--------
If you were using view model for retrieving and saving, most likely your view model will have responsibility for two different concerns.

1. Contain sufficient data for displaying and editing information
2. Contain sufficient data for changed information. 

For example, Dropdown control requires value and the list for editing, but you need only new value for saving. On the other hand, Save button may accept user comment so that you can record it on a log. In this case, UserComment property is useful only for saving but not for loading. 

View model contains all properties all the time and they are active depending on the context. You don't want to do this. Separate your Query model from your Command model.
