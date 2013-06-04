namespace PixelSenseLibrary.Events
{
    public delegate void GenericRoutedEventHandler<T>(object sender,GenericRoutedEventArgs<T> args);
    public delegate void GenericRoutedEventHandler<T, U>(object sender, GenericRoutedEventArgs<T, U> args);

}