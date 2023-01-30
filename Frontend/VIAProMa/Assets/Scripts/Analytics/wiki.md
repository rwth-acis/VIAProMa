# The telemetry feature of VIAProMa

This module enables you to add automatic data collection to features you implement or have already implemented in VIAProMa. The features are collected on a project base and persisted over saving. Telemetry can be en- and disabled and is stored on the VIAProMa backend from where it can be exported in file formats or to a learing record store (LRS).

## Adding a new telemetry feature

This implementation uses the observer pattern. [Learn the pattern](https://en.wikipedia.org/wiki/Observer_pattern) and [Read the docs](https://learn.microsoft.com/en-us/dotnet/api/system.iobservable-1).

1. Create a class to model the schema of the information you want to be logged in the `VIAProMa.Assets.Scripts.Analytics.LogTypes` namespace. Your class must inherit from the `Logpoint` class. All fields must have a public getter. The classes' type will from now on be refered to as the *log type*.
2. Find a class that has a method, which is executed every time the action you want to track occurs. This could for example be a button class and it's OnPressed() method.
3. Add a private field `observer` of the type `List<IObserver<LogType>>`.
4. Let this class implement the `IObservable<LogType>` interface, where `LogType` is the log type discussed in step 1.
5. Implement the `Subscribe()` method and the `Unsubscriber` subclass, as well as the `NotifyObservers()` method like shown in the example below. The method `EndTransmission()` is optional.
6. Think of a name for an observer type. For a button that edits an issue card the name `IssueEditingObserver` might be a good fit. In the example below, replace `ObserverType` with your selected name.

    Reference implementation:

    **!! Make sure to replace *LogType* with the type you created in step 1.**
    **!! There are more steps to complete below the example. Please do not forget them :)**

    ```csharp
    public class Button : IObserable<LogType>
    {
        // Your fields go here...

        private List<IObserver<LogType>> observers = new List<IObserver<LogType>>();

        // Your code goes here...

        // Implementation of necessary methods for the analytics module
        #region Observer Implemenations
        public IDisposable Subscribe(IObserver<LogType> observer)
        {
            if (! observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        public void NotifyObservers(LogType log)
        {
            if (SettingsManager.Instance.AnalyticsEnabled)
                foreach (var observer in observers)
                    observer.OnNext(log);
        }

        // This method is optional. If you don't need it, there is no need to implement it.
        public void EndTransmission()
        {
            foreach (var observer in observers.ToArray())
                if (observers.Contains(observer))
                    observer.OnCompleted();

            observers.Clear();
        }

        // This class must be implemented to enable an Observer to unsubscribe from an Observable.
        // The interface IObservable<...> requires the return type to implement the IDisposable interface which is satisfied by the Unsubscriber class.
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<LogType>>_observers;
            private IObserver<LogType> _observer;

            public Unsubscriber(List<IObserver<LogType>> observers, IObserver<LogType> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }


        class ObserverType : ObserverWrapper<LogType>.Observer
        {
            public ObserverType(IObservable<LogType> observable) : base(observable) { }
        }
        #endregion Observer Implemenations
    }
    ```

7. Go to the method that is executed every time the action you want to track occurs and add the following line, where you provide the constructor of you log type with the respective data. Again, make sure to replace *LogType* with your actual type name.

    ```csharp
    NotifyObservers(new LogType(/* Your necessary parameters */));
    ```

8. In your `Awake` method, add an observer to the observable object (`this`). Make sure to change `ObserverType` to the class name you thought of in step 6 and `LogType` to your custom log type. Note: The observable can be assigned an observer anywhere and any time *after* the instatiation of the observable. If you want to do the assignment somwhere else, for example to end the transmission once it is not necessary anymore or you want to unsubscribe the observer, this can be done there. You need a reference to the observable though and need to be sure that it is instantiated.

    ```csharp
    private void Awake()
        {
            // Add an observer for the analytics for this object.
            _ = new ObserverType(this); // TODO move observers to management class.

            // Your Awake code here...
        }
    ```

## Adding gaze detection to a GameObject

1. Implement a new component (a class) that must be added to all GameObjects of one kind that shall be gaze detectable. The class must inherit from the abstract class `MonoBehaviour` and implement the interface `IObservable<LogpointGazedAt>`.
2. The class must implement the interface as described above in step 5-8, where *LogType* must be replaced with `LogpointGazedAt`.
3. Add a private void returning method without any parameters that calls `NotifyObservers(new LogpointGazedAt(...));`.
4. The constructor `LogpointGazedAt(...)` requires the `LoggedObjectID` and `LoggedObjectType` of the action you want to log. The `LoggedObjectType` is a string you can set individually. If you are logging a user gazing at an issue card for example, the `LoggedObjectID` could be "IssueCard". The `LoggedObjectID` on the other hand is a GUID that is assigned to the object at TODO
5. Add the `EyeTrackingTarget` component to the game object (possibly prefab) you want to enable gaze detection for. Choose `Select` as Selection Action. On any of the callbacks called `On...`, click the plus, add the object itself as object and under function select the class you created in step 1 and the method you created in step 3.

The implementation could look something like this:

```csharp
public class GazableIssueCard : MonoBehaviour, IObservable<LogpointGazedAt>
{
    private List<IObserver<LogpointGazedAt>> observers = new List<IObserver<LogpointGazedAt>>();

    void Awake()
    {
        _ = new IssueCardGazingObserver(this);
    }

    public void GazeDetected()
    {
        string loggedObjectID = this.GetComponent<Serializer>().Id;
        LogpointGazedAt logpoint = new LogpointGazedAt(loggedObjectID, "IssueCard");
        NotifyObservers(logpoint);
    }
    
    // Implementation of necessary methods for the analytics module.
    #region Observer Implemenations
    // As shown in the example above.
    #endregion Observer Implemenations
}
```

## Exporting analytics data

TODO
