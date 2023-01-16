# How to add telemetry to a class

This implementation uses the observer pattern. [Learn the pattern](https://en.wikipedia.org/wiki/Observer_pattern) and [Read the docs](https://learn.microsoft.com/en-us/dotnet/api/system.iobservable-1).

1. Create a class for holding all information that shall be loged. This class must inherit from the `Logpoint` class in the namespace `VIAProMa.Assets.Scripts.Analytics.LogTypes` All fields must have a public getter. The classes' type will from now on be refered to as the *log type*.
2. Find a class that has a method, which is executed every time the action you want to track occurs. This could for example be a button class and it's OnPressed() method.
3. Add a private field observer of the type `List<IObserver<LogType>>`.
4. Let this class implement the *IObservable\<LogType>* interface, where *LogType* is the log type discussed in step 1. Note: If you see a backslash in the italic text, exclude it from the interface name. It is just necessary for Markdown formatting.
5. Implement the *Subscribe* method and the *Unsubscriber* subclass, as well as the *NotifyListeners()* method. The method *EndTransmission()* is optional.

    Reference implementation:

    **!! Make sure to replace *LogType* with the type you created in step 1.**

    ```csharp
    public class Button : IObserable<LogType>
    {
        // Your fields go here...

        private List<IObserver<LogType>> observers = new List<IObserver<LogType>>();

        // Your code go here...

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
            if (AnalyticsManager.Instance.AnalyticsEnabled)
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
        #endregion Observer Implemenations
    }
    ```

6. Go to the method that is executed every time the action you want to track occurs and add the following line, where you provide the constructor of you log type with the respective data. Again, make sure to replace *LogType* with your actual type name.

```csharp
NotifyObservers(new LogType(/* Your necessary parameters */));
```
