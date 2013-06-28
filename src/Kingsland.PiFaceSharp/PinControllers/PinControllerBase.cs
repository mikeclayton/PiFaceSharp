
namespace Kingsland.PiFaceSharp.PinControllers
{

    /// <summary>
    /// Implements a common base for classes that control a set of output pins
    /// on a PiFace Digital device.
    /// </summary>
    public abstract class PinControllerBase
    {

        #region Constructors

        /// <summary>
        /// Initialises a new pin controller and binds it to a specific PiFace Digital device.
        /// </summary>
        /// <param name="piface">
        /// A reference to the PiFace Digital device to control the output pins on.
        /// </param>
        protected PinControllerBase(IPiFaceDevice piface)
            : base()
        {
            this.PiFace = piface;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a reference to the PiFace Digital device this pin controller is bound to.
        /// </summary>
        public IPiFaceDevice PiFace
        {
            get;
            private set;
        }

        #endregion

    }

}
