using System;
using System.Drawing;
using civ2.Units;
using civ2.Enums;
using ExtensionMethods;

namespace civ2.Bitmaps
{
    public static partial class Draw
    {
        public static void UnitSprite(Graphics g, UnitType type, bool isSleeping, bool isFortified, int zoom, Point dest)
        {
            var image = ModifyImage.Resize(Images.Units[(int)type], zoom);
            if (!isSleeping)
                g.DrawImage(image, dest.X, dest.Y, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            else     // Sentry
                g.DrawImage(image, new Rectangle(dest.X, dest.Y, image.Width, image.Height),
                    0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ModifyImage.ConvertToGray());

            // Draw fortification
            if (isFortified) g.DrawImage(ModifyImage.Resize(Images.Fortified, zoom), dest.X, dest.Y);
        }

        public static void UnitShield(Graphics g, UnitType unitType, int ownerId, OrderType unitOrder, bool isStacked, int unitHP, int unitMaxHP, int zoom, Point dest)
        {
            // Draw unit shields. First determine if the shield is on the left or right side
            Point frontLoc = Images.UnitShieldLoc[(int)unitType];
            Point backLoc = frontLoc;
            if (frontLoc.X < 32) backLoc.X -= 4;
            else backLoc.X += 4;
            int shadowXoffset = frontLoc.X < 32 ? -1 : 1;
            // Scale locations according to zoom (shadow is always offset by 1)
            frontLoc.X = Ext.ZoomScale(frontLoc.X, zoom);
            frontLoc.Y = Ext.ZoomScale(frontLoc.Y, zoom);
            backLoc.X = Ext.ZoomScale(backLoc.X, zoom);
            backLoc.Y = Ext.ZoomScale(backLoc.Y, zoom);

            // If unit stacked --> draw back shield with its shadow
            var shadow = ModifyImage.Resize(Images.ShieldShadow, zoom);
            if (isStacked)
            {
                // Back shield shadow
                g.DrawImage(shadow,
                            new Rectangle(dest.X + backLoc.X + shadowXoffset, dest.Y + backLoc.Y + 1, shadow.Width, shadow.Height),
                            0, 0, shadow.Width, shadow.Height, GraphicsUnit.Pixel);
                // Back shield
                var back = ModifyImage.Resize(Images.ShieldBack[ownerId], zoom);
                g.DrawImage(back,
                            new Rectangle(dest.X + backLoc.X, dest.Y + backLoc.Y, back.Width, back.Height),
                            0, 0, back.Width, back.Height, GraphicsUnit.Pixel);
            }

            // Front shield shadow
            g.DrawImage(shadow, dest.X + frontLoc.X + shadowXoffset, dest.Y + frontLoc.Y, new Rectangle(0, 0, shadow.Width, shadow.Height), GraphicsUnit.Pixel);

            // Front shield
            var front = ModifyImage.Resize(Images.ShieldFront[ownerId], zoom);
            g.DrawImage(front, dest.X + frontLoc.X, dest.Y + frontLoc.Y, new Rectangle(0, 0, front.Width, front.Height), GraphicsUnit.Pixel);

            // Determine hitpoints bar size
            int hpBarX = (int)Math.Floor((float)unitHP * 12 / unitMaxHP);
            Color hpColor;
            if (hpBarX <= 3)
                hpColor = Color.FromArgb(243, 0, 0); // Red
            else if (hpBarX >= 4 && hpBarX <= 8)
                hpColor = Color.FromArgb(255, 223, 79);  // Yellow
            else
                hpColor = Color.FromArgb(87, 171, 39);   // Green
            
            // Draw black background for hitpoints bar
            g.FillRectangle(new SolidBrush(Color.Black),
                new Rectangle(dest.X + frontLoc.X, dest.Y + frontLoc.Y, Ext.ZoomScale(12, zoom), Ext.ZoomScale(7, zoom)));

            // Draw hitpoints bar
            g.FillRectangle(new SolidBrush(hpColor),
                new Rectangle(dest.X + frontLoc.X, dest.Y + frontLoc.Y + Ext.ZoomScale(2, zoom), Ext.ZoomScale(hpBarX, zoom), Ext.ZoomScale(3, zoom)));

            // Text on front shield
            using var sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            string shieldText;
            switch (unitOrder)
            {
                case OrderType.Fortify:
                case OrderType.Fortified: shieldText = "F"; break;
                case OrderType.Sleep: shieldText = "S"; break;
                case OrderType.BuildFortress: shieldText = "F"; break;
                case OrderType.BuildRoad: shieldText = "R"; break;
                case OrderType.BuildIrrigation: shieldText = "I"; break;
                case OrderType.BuildMine: shieldText = "m"; break;
                case OrderType.Transform: shieldText = "O"; break;
                case OrderType.CleanPollution: shieldText = "p"; break;
                case OrderType.BuildAirbase: shieldText = "E"; break;
                case OrderType.GoTo: shieldText = "G"; break;
                case OrderType.NoOrders: shieldText = "-"; break;
                default: shieldText = "-"; break;
            }
            g.DrawString(shieldText, new Font("Arial", Ext.ZoomScale(8, zoom)), new SolidBrush(Color.Black),
                dest.X + frontLoc.X + Ext.ZoomScale(6, zoom), dest.Y + frontLoc.Y + Ext.ZoomScale(12, zoom), sf);
        }

        public static void Unit(Graphics g, IUnit unit, bool isStacked, int zoom, Point dest)
        {
            UnitShield(g, unit.Type, unit.Owner.Id, unit.Order, isStacked, unit.HitPoints, unit.MaxHitpoints, zoom, dest);
            UnitSprite(g, unit.Type, unit.Order == OrderType.Sleep, unit.Order == OrderType.Fortified, zoom, dest);
        }

        // Draw unit type (not in game units and their stats, just unit types for e.g. defense minister statistics)
        public static Bitmap UnitType(int Id, int civId)  //Id = order from RULES.TXT, civId = id of civ (0=barbarian)
        {
            var square = new Bitmap(64, 48);     //define a bitmap for drawing

            using (var g = Graphics.FromImage(square))
            {
                using var sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                //draw unit shields
                //First determine if the shield is on the left or right side
                int firstShieldXLoc = Images.UnitShieldLoc[Id].X;
                int secondShieldXLoc = firstShieldXLoc;
                int secondShieldBorderXLoc;
                int borderShieldOffset;
                if (firstShieldXLoc < 32)
                {
                    borderShieldOffset = -1;
                    secondShieldXLoc -= 4;
                    secondShieldBorderXLoc = secondShieldXLoc - 1;
                }
                else
                {
                    borderShieldOffset = 1;
                    secondShieldXLoc += 4;
                    secondShieldBorderXLoc = secondShieldXLoc + 1;
                }
                //graphics.DrawImage(Images.UnitShieldShadow, Images.unitShieldLocation[Id, 0] + borderShieldOffset, Images.unitShieldLocation[Id, 1]); //shield shadow
                //graphics.DrawImage(Images.UnitShield[civId], Images.unitShieldLocation[Id, 0], Images.unitShieldLocation[Id, 1]); //main shield
                //graphics.DrawString("-", new Font("Arial", 8.0f), new SolidBrush(Color.Black), Images.unitShieldLocation[Id, 0] + 6, Images.unitShieldLocation[Id, 1] + 12, sf);    //Action on shield
                g.DrawImage(Images.Units[Id], 0, 0);    //draw unit
            }

            return square;
        }

    }
}
