using ResuMPServer.Constant;
using Shared;

namespace ResuMPServer
{
    public class TextLabel : Entity
    {
        internal TextLabel(API father, NetHandle handle) : base(father, handle)
        {
        }

        #region Properties

        public string Text
        {
            get => GetTextLabelText();
            set => SetTextLabelText(value);
        }

        public Color Color
        {
            get => GetTextLabelColor();
            set => SetTextLabelColor(value.red, value.green, value.blue, value.alpha);
        }

        public bool Seethrough
        {
            get => GetTextLabelSeethrough();
            set { SetTextLabelSeethrough(value); }
        }

        public float Range
        {
            get => GetTextLabelRange();
            set => SetTextLabelRange(value);
        }

        #endregion

        #region Methods
        public void SetTextLabelText(string newText)
        {
            if (DoesEntityExist())
            {
                ((TextLabelProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Text = newText;

                var delta = new TextLabelProperties();
                delta.Text = newText;
                GameServer.UpdateEntityInfo(Value, EntityType.TextLabel, delta);
            }
        }

        public string GetTextLabelText()
        {
            if (DoesEntityExist())
            {
                return ((TextLabelProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Text;
            }

            return null;
        }

        public void SetTextLabelRange(float newRange)
        {
            if (DoesEntityExist())
            {
                ((TextLabelProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Range = newRange;

                var delta = new TextLabelProperties();
                delta.Range = newRange;
                GameServer.UpdateEntityInfo(Value, EntityType.TextLabel, delta);
            }
        }

        public float GetTextLabelRange()
        {
            if (DoesEntityExist())
            {
                return ((TextLabelProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Range;
            }

            return 0;
        }

        public void SetTextLabelColor(int red, int green, int blue, int alpha)
        {
            if (DoesEntityExist())
            {
                ((TextLabelProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Alpha = (byte)alpha;
                ((TextLabelProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Red = red;
                ((TextLabelProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Green = green;
                ((TextLabelProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Blue = blue;

                var delta = new TextLabelProperties();
                delta.Alpha = (byte)alpha;
                delta.Red = red;
                delta.Green = green;
                delta.Blue = blue;
                GameServer.UpdateEntityInfo(Value, EntityType.TextLabel, delta);
            }
        }

        public Color GetTextLabelColor()
        {
            Color output = new Color();

            if (DoesEntityExist())
            {
                output.alpha = ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Alpha;
                output.red = ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Red;
                output.green = ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Green;
                output.blue = ((MarkerProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).Blue;
            }

            return output;
        }

        public void SetTextLabelSeethrough(bool seethrough)
        {
            if (DoesEntityExist())
            {
                ((TextLabelProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).EntitySeethrough =
                    seethrough;

                var delta = new TextLabelProperties();
                delta.EntitySeethrough = seethrough;
                GameServer.UpdateEntityInfo(Value, EntityType.TextLabel, delta);
            }
        }

        public bool GetTextLabelSeethrough()
        {
            if (DoesEntityExist())
            {
                return ((TextLabelProperties)Program.ServerInstance.NetEntityHandler.ServerEntities[Value]).EntitySeethrough;
            }
            return false;
        }
        #endregion
    }
}