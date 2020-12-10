using System;
using System.Drawing;
using civ2.Units;
using civ2.Enums;

namespace civ2.Bitmaps
{
    public partial class Draw
    {
        public static Bitmap Unit(IUnit unit, bool drawInStack, int zoom)
        {
            // Define a bitmap for drawing
            Bitmap unitPic = new Bitmap(64, 48);     

            using (Graphics graphics = Graphics.FromImage(unitPic))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                string shieldText;
                switch (unit.Order)
                {
                    case OrderType.Fortify: shieldText = "F"; break;
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

                // Draw unit shields. First determine if the shield is on the left or right side
                int firstShieldXLoc = Images.unitShieldLocation[(int)unit.Type, 0];
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

                // Determine hitpoints bar size
                int hitpointsBarX = (int)Math.Floor((float)unit.HitPoints * 12 / unit.MaxHitpoints);
                Color hitpointsColor;
                if (hitpointsBarX <= 3)
                    hitpointsColor = Color.FromArgb(243, 0, 0);
                else if (hitpointsBarX >= 4 && hitpointsBarX <= 8)
                    hitpointsColor = Color.FromArgb(255, 223, 79);
                else
                    hitpointsColor = Color.FromArgb(87, 171, 39);

                // Draw dark shield if unit is stacked on top of others
                if (drawInStack)
                {
                    graphics.DrawImage(Images.UnitShieldShadow, secondShieldBorderXLoc, Images.unitShieldLocation[(int)unit.Type, 1]); // Shield shadow
                    graphics.DrawImage(Images.NoBorderUnitShield[unit.Owner.Id], secondShieldXLoc, Images.unitShieldLocation[(int)unit.Type, 1]);   // Dark shield
                }

                // Shield shadow
                graphics.DrawImage(Images.UnitShieldShadow, Images.unitShieldLocation[(int)unit.Type, 0] + borderShieldOffset, Images.unitShieldLocation[(int)unit.Type, 1] - borderShieldOffset);

                // Main shield
                graphics.DrawImage(Images.UnitShield[unit.Owner.Id], Images.unitShieldLocation[(int)unit.Type, 0], Images.unitShieldLocation[(int)unit.Type, 1]);

                // Draw black background for hitpoints bar
                graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(Images.unitShieldLocation[(int)unit.Type, 0], Images.unitShieldLocation[(int)unit.Type, 1] + 2, 12, 3));

                // Draw hitpoints bar
                graphics.FillRectangle(new SolidBrush(hitpointsColor), new Rectangle(Images.unitShieldLocation[(int)unit.Type, 0], Images.unitShieldLocation[(int)unit.Type, 1] + 2, hitpointsBarX, 3));

                // Action on shield
                graphics.DrawString(shieldText, new Font("Arial", 6.5f), new SolidBrush(Color.Black), Images.unitShieldLocation[(int)unit.Type, 0] + 7, Images.unitShieldLocation[(int)unit.Type, 1] + 12, sf);

                // Draw unit
                if (unit.Order != OrderType.Sleep)
                {
                    graphics.DrawImage(Images.Units[(int)unit.Type], 0, 0);
                }
                // Draw sentry unit
                else
                {
                    graphics.DrawImage(Images.Units[(int)unit.Type], new Rectangle(0, 0, 64, 48), 0, 0, 64, 48, GraphicsUnit.Pixel, ModifyImage.ConvertToGray());
                }

                // Draw fortification
                if (unit.Order == OrderType.Fortified) graphics.DrawImage(Images.Fortified, 0, 0);

                sf.Dispose();
            }

            // Resize image if required
            unitPic = ModifyImage.ResizeImage(unitPic, zoom);

            return unitPic;
        }

        //Draw unit type (not in game units and their stats, just unit types for e.g. defense minister statistics)
        public Bitmap UnitType(int Id, int civId)  //Id = order from RULES.TXT, civId = id of civ (0=barbarian)
        {
            Bitmap square = new Bitmap(64, 48);     //define a bitmap for drawing

            using (Graphics graphics = Graphics.FromImage(square))
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                //draw unit shields
                //First determine if the shield is on the left or right side
                int firstShieldXLoc = Images.unitShieldLocation[Id, 0];
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
                graphics.DrawImage(Images.UnitShieldShadow, Images.unitShieldLocation[Id, 0] + borderShieldOffset, Images.unitShieldLocation[Id, 1]); //shield shadow
                graphics.DrawImage(Images.UnitShield[civId], Images.unitShieldLocation[Id, 0], Images.unitShieldLocation[Id, 1]); //main shield
                graphics.DrawString("-", new Font("Arial", 8.0f), new SolidBrush(Color.Black), Images.unitShieldLocation[Id, 0] + 6, Images.unitShieldLocation[Id, 1] + 12, sf);    //Action on shield
                graphics.DrawImage(Images.Units[Id], 0, 0);    //draw unit
                sf.Dispose();
            }

            return square;
        }

    }
}
