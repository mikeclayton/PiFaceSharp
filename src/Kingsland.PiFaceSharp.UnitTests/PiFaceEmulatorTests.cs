using NUnit.Framework;

namespace Kingsland.PiFaceSharp.UnitTests
{

    public class PiFaceEmulator
    {

        [TestFixture]
        public class SetOutputPinStatesMethod
        {

            [TestCase(0)]
            public void PropertySetAppliesCorrectBitMask(byte bitMask)
            {
                var piface = new Emulators.PiFaceEmulator();
                piface.SetOutputPinStates(bitMask);
                Assert.AreEqual(piface.GetOutputPinStates(), bitMask);
            }

            // all off
            [TestCase(0, new[] { false, false, false, false, false, false, false, false })]
            // all on
            [TestCase(255, new[] { true, true, true, true, true, true, true, true })]
            // each pin on individually
            [TestCase(1, new[] { true, false, false, false, false, false, false, false })]
            [TestCase(2, new[] { false, true, false, false, false, false, false, false })]
            [TestCase(4, new[] { false, false, true, false, false, false, false, false })]
            [TestCase(8, new[] { false, false, false, true, false, false, false, false })]
            [TestCase(16, new[] { false, false, false, false, true, false, false, false })]
            [TestCase(32, new[] { false, false, false, false, false, true, false, false })]
            [TestCase(64, new[] { false, false, false, false, false, false, true, false })]
            [TestCase(128, new[] { false, false, false, false, false, false, false, true })]
            // arbitrary combinations
            [TestCase(192, new[] { false, false, false, false, false, false, true, true })]
            [TestCase(48, new[] { false, false, false, false, true, true, false, false })]
            [TestCase(12, new[] { false, false, true, true, false, false, false, false })]
            [TestCase(3, new[] { true, true, false, false, false, false, false, false })]
            [TestCase(240, new[] { false, false, false, false, true, true, true, true })]
            [TestCase(15, new[] { true, true, true, true, false, false, false, false })]
            [TestCase(85, new[] { true, false, true, false, true, false, true, false })]
            [TestCase(170, new[] { false, true, false, true, false, true, false, true })]
            public void PropertySetAppliesCorrectPinStates(byte bitMask, bool[] pinStates)
            {
                var piface = new Emulators.PiFaceEmulator();
                piface.SetOutputPinStates(bitMask);
                for (var i = (byte)0; i < pinStates.Length; i++)
                {
                    Assert.AreEqual(pinStates[i], piface.GetOutputPinState(i));
                }
            }
            
        }

        [TestFixture]
        public class SetInputPinStatesMethod
        {

            [TestCase(0)]
            public void PropertySetAppliesCorrectBitMask(byte bitMask)
            {
                var piface = new Emulators.PiFaceEmulator();
                piface.SetInputPinStates(bitMask);
                Assert.AreEqual(piface.GetInputPinStates(), bitMask);
            }

            // all off
            [TestCase(0, new[] { true, true, true, true, true, true, true, true })]
            // all on
            [TestCase(255, new[] { false, false, false, false, false, false, false, false })]
            // each pin on individually
            [TestCase(1, new[] { false, true, true, true, true, true, true, true })]
            [TestCase(2, new[] { true, false, true, true, true, true, true, true })]
            [TestCase(4, new[] { true, true, false, true, true, true, true, true })]
            [TestCase(8, new[] { true, true, true, false, true, true, true, true })]
            [TestCase(16, new[] { true, true, true, true, false, true, true, true })]
            [TestCase(32, new[] { true, true, true, true, true, false, true, true })]
            [TestCase(64, new[] { true, true, true, true, true, true, false, true })]
            [TestCase(128, new[] { true, true, true, true, true, true, true, false })]
            // arbitrary combinations
            [TestCase(192, new[] { true, true, true, true, true, true, false, false })]
            [TestCase(48, new[] { true, true, true, true, false, false, true, true })]
            [TestCase(12, new[] { true, true, false, false, true, true, true, true })]
            [TestCase(3, new[] { false, false, true, true, true, true, true, true })]
            [TestCase(240, new[] { true, true, true, true, false, false, false, false })]
            [TestCase(15, new[] { false, false, false, false, true, true, true, true })]
            [TestCase(85, new[] { false, true, false, true, false, true, false, true })]
            [TestCase(170, new[] { true, false, true, false, true, false, true, false })]
            public void PropertySetAppliesCorrectPinStates(byte bitMask, bool[] pinStates)
            {
                // added delay to test team city real-time progress updates with slow tests
                System.Threading.Thread.Sleep(10000);
                var piface = new Emulators.PiFaceEmulator();
                piface.SetInputPinStates(bitMask);
                for (var i = (byte)0; i < pinStates.Length; i++)
                {
                    Assert.AreEqual(pinStates[i], piface.GetInputPinState(i));
                }
            }

        }

    }

}
