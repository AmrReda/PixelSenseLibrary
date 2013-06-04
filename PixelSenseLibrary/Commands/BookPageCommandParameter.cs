using PixelSenseLibrary.Controls.ItemsCollections;
using PixelSenseLibrary.Enums;

namespace PixelSenseLibrary.Commands
{
    public class BookPageCommandParameter
    {
        // Fields
        private int m_nDropAnimationDuration = BookPage.s_nANIMATION_DURATION;
        private int m_nTurnAnimationDuration = BookPage.s_nANIMATION_DURATION;
        private CornerOrigin m_refCornerOrigin;

        // Properties
        public CornerOrigin CornerOrigin
        {
            get
            {
                return this.m_refCornerOrigin;
            }
            set
            {
                this.m_refCornerOrigin = value;
            }
        }

        public int DropAnimationDuration
        {
            get
            {
                return this.m_nDropAnimationDuration;
            }
            set
            {
                this.m_nDropAnimationDuration = value;
            }
        }

        public int TurnAnimationDuration
        {
            get
            {
                return this.m_nTurnAnimationDuration;
            }
            set
            {
                this.m_nTurnAnimationDuration = value;
            }
        }

    }
}