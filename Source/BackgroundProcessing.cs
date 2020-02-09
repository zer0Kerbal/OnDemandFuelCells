/*
namespace ODFC
{

    #region Background Processing Code
    /// <summary>
    /// Background Processing FixedBackgroundUpdate (A)
    /// </summary>
    /// <param name="v"></param>
    /// <param name="partFlightID"></param>
    /// <param name="data"></param>
    public static void FixedBackgroundUpdate(Vessel v, uint partFlightID, ref System.Object data)
    {
        Log.dbg("ODFC BackgroundProcessing: FixedBackgroundUpdate overload a");
    }

    /// <summary>
    /// Background Processing FixedBackgroundUpdate (B)
    /// </summary>
    /// <param name="v"></param>
    /// <param name="partFlightID"></param>
    /// <param name="resourceRequest"></param>
    /// <param name="data"></param>
    public static void FixedBackgroundUpdate(Vessel v, uint partFlightID, Func<Vessel, float, string, float> resourceRequest, ref System.Object data)
    {
        Log.dbg("ODFC BackgroundProcessing: FixedBackgroundUpdate overload b");
    }

    *//* These two functions are the 'simple' and 'complex' background update functions. If you implement either of them, BackgroundProcessing will call it 
     * at FixedUpdate() intervals (if you implement both, only the complex version will get called). The function will only be called for unloaded vessels, 
     * and it will be called once per part per partmodule type - so if you have more than one of the same PartModule on the same part you'll only get one 
     * update for all of those PartModules. Vessel v is the Vessel object that you should update - be careful, it's quite likely unloaded and very little 
     * of it is there. partFlightID is the flightID of the Part this update was associated with. resourceRequest is a function that provides an analog of 
     * Part.RequestResource. It takes a vessel, an amount of resource to take, and a resource name, and returns the amount of resource that you got. Like 
     * Part.RequestResource, you can ask for a negative amount of some resource to fill up the tanks. The resource is consumed as if it can be reached from the entire vessel - be very careful with resources like liquid fuel that should only flow through crossfeeds. data is arbitrary per-part-per-partmodule-type storage - you can stash anything you want there, and it will persist between FixedBackgroundUpdate calls. 
    *//*

    /// <summary>
    /// BackgroundLoad 
    /// </summary>
    /// <param name="v"></param>
    /// <param name="partFlightId"></param>
    /// <param name="data"></param>
    public static void BackgroundLoad(Vessel v, uint partFlightId, ref System.Object data)
    {
        Log.dbg("ODFC BackgroundProcessing: Background Load");
    }

    *//* This function will be called once prior to FixedBackgroundUpdate, and it gives you a chance to load data out of the ConfigNodes on the vessel's 
     * protovessel into the storage BackgroundProcessing manages for you.
    *//*

    /// <summary>
    /// BackgroundSave
    /// </summary>
    /// <param name="v"></param>
    /// <param name="partFlightId"></param>
    /// <param name="data"></param>
    public static void BackgroundSave(Vessel v, uint partFlightId, System.Object data)
    {
        Log.dbg("ODFC BackgroundProcessing: Background Save");
    }

    *//* This function will be called prior to the game scene changing or the game otherwise being saved.Use it to persist background data to the vessel's confignodes.
     * Note that System.Object data is *not* a ref type here, unlike the other functions that have it as an argument.
    *//*

    /// <summary>
    /// GetInterestingResources
    /// </summary>
    /// <returns></returns>
    public static List<string> GetInterestingResources()
    {
        Log.dbg("ODFC BackgroundProcessing: GetInterestingResources");
        return null;
    }

    *//* Implement this function to return a list of resources that your PartModule would like BackgroundProcessing to handle in the background. It's okay if multiple 
     * PartModules say a given resource is interesting.
    *//*

    /// <summary>
    /// GetBackgroundResourceCount
    /// </summary>
    /// <returns></returns>
    public static int GetBackgroundResourceCount()
    {
        Log.dbg("ODFC BackgroundProcessing: GetBackgroundResourceCount");
        return 0;
    }

    /// <summary>
    /// GetBackgroundResource
    /// </summary>
    /// <param name="index"></param>
    /// <param name="resourceName"></param>
    /// <param name="resourceRate"></param>
    public static void GetBackgroundResource(int index, out string resourceName, out float resourceRate)
    {
        resourceName = "none";
        resourceRate = 0f;
        Log.dbg("ODFC BackgroundProcessing: GetBackgroundResource: " + resourceName + " : " + resourceRate);
        return;
    }

        *//* Implement these functions to inform BackgroundProcessing that your PartModule should be considered to produce or consume a resource in the background.
         * GetBackgroundResourceCount() should return the number of different resources that your PartModule produces/consumes.GetBackgroundResource() will then 
         * be called with each index from 0 up to one less than the count you indicated, and you should set resourceName and resourceRate to the appropriate values 
         * for the index-th resource your PartModule produces.resourceRate is the amount of resource your part should produce each second - if your part consumes 
         * resources, it should be negative.If your part only consumes or produces resources in some situation, it's better to implement the complex background 
         * update and use the resource request function.

        How do I distribute a mod that uses BackgroundProcessing?

          * Having multiple copies of the BackgroundProcessing DLL in a GameData directory is fine - only the most recent version will run.If your mod absolutely 
          * needs BackgroundProcessing present to be useful, consider including the BackgroundProcessing DLL in your mod's zip file, the same way ModuleManager is 
          * handled.

        If BackgroundProcessing isn't central to your mod, feel free not to distribute it at all. If players don't have this mod installed, they just won't get your off-rails features.

        Are there any caveats when writing code that uses BackgroundProcessing?

            * BackgroundProcessing works optimally with PartModules that are present in prefab parts.The list of modules that has BackgroundProcessing handling is 
            * constructed by walking all the AvailablePart objects in PartLoader.LoadedPartLists at the main menu.If your partmodule isn't in that set, very little will 
            * work. Similarly, if your PartModule is added to a part's module list after part instantiation, and isn't present in the part's prefab module list, resource 
            * handling will likely not work. This is all only relevant if your PartModule is added dynamically to parts, after construction. It isn't relevant to PartModules 
            * that are present in config files, or to PartModules added by ModuleManager (which appropriately modifies prefab parts). The takeaway is that if you intend 
            * to dynamically add PartModules to parts, and those dynamically-added PartModules should have BackgroundProcessing behaviour, make sure you add them to the 
            * appropriate part prefab before the main menu, like ModuleManager.

        Edited August 3, 2016 by jamespicone
        *//*

#endregion
}*/