public static class LocationManager
{
    public static double SelectedLatitude { get; set; }
    public static double SelectedLongitude { get; set; }

    public static bool LocationSelected => SelectedLatitude != 0 || SelectedLongitude != 0;
}
