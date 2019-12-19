
namespace ODFC
{
    class IRRescale
    {
        class MyUpdater : TweakScale.IRescalable<ODFC>
        {
            private readonly ODFC pm;

            public MyUpdater(ODFC pm)
            {
                this.pm = pm;
            }

            public void OnRescale(TweakScale.ScalingFactor factor)
            {
                pm.OnRescale(factor.relative);//. .absolute);
            }
        }
    }
}