﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Controls
{
    /// <summary>
    /// TreeView
    /// </summary>
    public class TreeViewCus : TreeView
    {
        /// <summary>
        /// Polozenie myszki
        /// </summary>
        public int TreeX;

        /// <summary>
        /// Metody przeciwdzaiłające migotaniu
        /// </summary>
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;

        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        /// <summary>
        /// Konstruktor
        /// </summary>
        public TreeViewCus()
        {
            MouseMove += new MouseEventHandler(TreeViewCus_MouseMove);
            BeforeCollapse += new TreeViewCancelEventHandler(TreeViewCus_BeforeCollapse);
            BeforeExpand += new TreeViewCancelEventHandler(TreeViewCus_BeforeExpand);
        }

        /// <summary>
        /// Przeciwko migotaniu. Wlaczenie DoubleBuffered Tryb
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }

        /// <summary>
        /// Przed rozwinieciem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewCus_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            //Blokada rozwienicia przy kliknieciu podwojnym
            if (TreeX > e.Node.Bounds.Left) e.Cancel = true;
        }

        /// <summary>
        /// Przed zwinieciem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewCus_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            //Blokada rozwienicia przy kliknieciu podwojnym
            if (TreeX > e.Node.Bounds.Left) e.Cancel = true;
        }

        /// <summary>
        /// Zmiana polozenia myszki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewCus_MouseMove(object sender, MouseEventArgs e)
        {
            TreeX = e.X;
        }
    }

    /// <summary>
    /// DataGrid
    /// </summary>
    public class DataGridViewCus : DataGridView
    {
        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        /// <summary>
        /// Przeciwko migotaniu. Wlaczenie DoubleBuffered Tryb
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            SendMessage(this.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
            base.OnHandleCreated(e);
        }
    }

    /// <summary>
    /// Drawing Button
    /// </summary>
    public class DataGridViewButtonCellCus : DataGridViewButtonCell
    {
        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates elementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            //ButtonRenderer.DrawButton(graphics, cellBounds, formattedValue.ToString(), new Font("Tahoma", 9.0f, FontStyle.Bold), true, System.Windows.Forms.VisualStyles.PushButtonState.Normal);
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
        }
    }

    /// <summary>
    /// Dodatkowa zakladka dodajaca zdarzenia
    /// </summary>
    public class EventPropertyTab : PropertyTab
    {
        /// <summary>
        /// Formatowanie wlasciwosci
        /// </summary>
        /// <param name="component"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            //Pobranie wlasciowosci obke
            PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<PropertyDescriptor> filtereds = new List<PropertyDescriptor>();
            PropertyDescriptorCollection propertyDescriptions = TypeDescriptor.GetProperties(component, attributes);

            Type EventAttributeType = typeof(CusEventPropertyAttribute);
            for (int i = 0; i < properties.Length; i++)
            {
                //Sprawdzenie czy atrybut to event
                if (properties[i].IsDefined(EventAttributeType, true))
                    filtereds.Add(propertyDescriptions[properties[i].Name]);
            }

            // now add a custom property descriptor to show hidden properties
            PropertyDescriptorCollection coll = new PropertyDescriptorCollection(filtereds.ToArray());

            return coll;
        }

        /// <summary>
        /// Wyswietlania bitmapa
        /// </summary>
        public override Bitmap Bitmap
        {
            get
            {
                return Zasoby.scripts_icon;
            }
        }

        /// <summary>
        /// Nazwa Tab
        /// </summary>
        public override string TabName
        {
            get { return "Event"; }
        }
    }

    /// <summary>
    ///  This attribute is using to indicate that a property is a filtered property for us.
    ///
    ///  Properties which has this attribute will be shown in our Custom Property Tab Page
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CusEventPropertyAttribute : Attribute
    {
    }

    /// <summary>
    /// Edytor scryptu
    /// </summary>
    public class ScEditor : UITypeEditor
    {
        /// <summary>
        /// Typ edytora
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Edycja
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            UIScript ui = new UIScript
            {
                Dock = DockStyle.Fill
            };

            if (value != null)
                ui.editor.Text = value.ToString();
            else
                ui.editor.Text = " ";

            ui.ShowDialog();

            value = ui.editor.Text;

            return base.EditValue(context, provider, value);
        }
    }
}

namespace XButton
{
    #region ENUM

    public enum Theme
    {
        MSOffice2010_BLUE = 1,
        MSOffice2010_WHITE = 2,
        MSOffice2010_RED = 3,
        MSOffice2010_Green = 4,
        MSOffice2010_Pink = 5,
        MSOffice2010_Yellow = 6,
        MSOffice2010_Publisher = 7
    }

    #endregion ENUM

    #region COLOR TABLE

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Colortable
    {
        #region Static Color Tables

        private static Office2010Blue office2010blu = new Office2010Blue();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public static Colortable Office2010Blue
        {
            get { return office2010blu; }
        }

        private static Office2010Green office2010gr = new Office2010Green();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public static Colortable Office2010Green
        {
            get { return office2010gr; }
        }

        private static Office2010Red office2010rd = new Office2010Red();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public static Colortable Office2010Red
        {
            get { return office2010rd; }
        }

        private static Office2010Pink office2010pk = new Office2010Pink();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public static Colortable Office2010Pink
        {
            get { return office2010pk; }
        }

        private static Office2010Yellow office2010yl = new Office2010Yellow();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public static Colortable Office2010Yellow
        {
            get { return office2010yl; }
        }

        private static Office2010White office2010wt = new Office2010White();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public static Colortable Office2010White
        {
            get { return office2010wt; }
        }

        private static Office2010Publisher office2010pb = new Office2010Publisher();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public static Colortable Office2010Publisher
        {
            get { return office2010pb; }
        }

        #endregion Static Color Tables

        #region Custom Properties

        private Color textColor = Color.White;
        private Color selectedTextColor = Color.FromArgb(30, 57, 91);
        private Color OverTextColor = Color.FromArgb(30, 57, 91);
        private Color borderColor = Color.FromArgb(31, 72, 161);
        private Color innerborderColor = Color.FromArgb(68, 135, 228);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color SelectedTextColor
        {
            get { return selectedTextColor; }
            set { selectedTextColor = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color HoverTextColor
        {
            get { return OverTextColor; }
            set { OverTextColor = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color BorderColor1
        {
            get { return borderColor; }
            set { borderColor = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color BorderColor2
        {
            get { return innerborderColor; }
            set { innerborderColor = value; }
        }

        #endregion Custom Properties

        #region Button Normal

        private Color buttonNormalBegin = Color.FromArgb(31, 72, 161);
        private Color buttonNormalMiddleBegin = Color.FromArgb(68, 135, 228);
        private Color buttonNormalMiddleEnd = Color.FromArgb(41, 97, 181);
        private Color buttonNormalEnd = Color.FromArgb(62, 125, 219);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonNormalColor1
        {
            get { return buttonNormalBegin; }
            set { buttonNormalBegin = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonNormalColor2
        {
            get { return buttonNormalMiddleBegin; }
            set { buttonNormalMiddleBegin = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonNormalColor3
        {
            get { return buttonNormalMiddleEnd; }
            set { buttonNormalMiddleEnd = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonNormalColor4
        {
            get { return buttonNormalEnd; }
            set { buttonNormalEnd = value; }
        }

        #endregion Button Normal

        #region Button Selected

        private Color buttonSelectedBegin = Color.FromArgb(236, 199, 87);
        private Color buttonSelectedMiddleBegin = Color.FromArgb(252, 243, 215);
        private Color buttonSelectedMiddleEnd = Color.FromArgb(255, 229, 117);
        private Color buttonSelectedEnd = Color.FromArgb(255, 216, 107);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonSelectedColor1
        {
            get { return buttonSelectedBegin; }
            set { buttonSelectedBegin = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonSelectedColor2
        {
            get { return buttonSelectedMiddleBegin; }
            set { buttonSelectedMiddleBegin = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonSelectedColor3
        {
            get { return buttonSelectedMiddleEnd; }
            set { buttonSelectedMiddleEnd = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonSelectedColor4
        {
            get { return buttonSelectedEnd; }
            set { buttonSelectedEnd = value; }
        }

        #endregion Button Selected

        #region Button Mouse Over

        private Color buttonMouseOverBegin = Color.FromArgb(236, 199, 87);
        private Color buttonMouseOverMiddleBegin = Color.FromArgb(252, 243, 215);
        private Color buttonMouseOverMiddleEnd = Color.FromArgb(249, 225, 137);
        private Color buttonMouseOverEnd = Color.FromArgb(251, 249, 224);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonMouseOverColor1
        {
            get { return buttonMouseOverBegin; }
            set { buttonMouseOverBegin = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonMouseOverColor2
        {
            get { return buttonMouseOverMiddleBegin; }
            set { buttonMouseOverMiddleBegin = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonMouseOverColor3
        {
            get { return buttonMouseOverMiddleEnd; }
            set { buttonMouseOverMiddleEnd = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ButtonMouseOverColor4
        {
            get { return buttonMouseOverEnd; }
            set { buttonMouseOverEnd = value; }
        }

        #endregion Button Mouse Over
    }

    #endregion COLOR TABLE

    #region Office 2010 Blue

    public class Office2010Blue : Colortable
    {
        public Office2010Blue()
        {
            // Border Color

            this.BorderColor1 = Color.FromArgb(31, 72, 161);
            this.BorderColor2 = Color.FromArgb(68, 135, 228);

            // Button Text Color

            this.TextColor = Color.White;
            this.SelectedTextColor = Color.FromArgb(30, 57, 91);
            this.HoverTextColor = Color.FromArgb(30, 57, 91);

            // Button normal color

            this.ButtonNormalColor1 = Color.FromArgb(31, 72, 161);
            this.ButtonNormalColor2 = Color.FromArgb(68, 135, 228);
            this.ButtonNormalColor3 = Color.FromArgb(41, 97, 181);
            this.ButtonNormalColor4 = Color.FromArgb(62, 125, 219);

            // Button mouseover color

            this.ButtonMouseOverColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonMouseOverColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonMouseOverColor3 = Color.FromArgb(249, 225, 137);
            this.ButtonMouseOverColor4 = Color.FromArgb(251, 249, 224);

            // Button selected color

            this.ButtonSelectedColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonSelectedColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonSelectedColor3 = Color.FromArgb(255, 229, 117);
            this.ButtonSelectedColor4 = Color.FromArgb(255, 216, 107);
        }

        public override string ToString()
        {
            return "Office2010Blue";
        }
    }

    #endregion Office 2010 Blue

    #region Office 2010 GREEN

    public class Office2010Green : Colortable
    {
        public Office2010Green()
        {
            // Border Color

            this.BorderColor1 = Color.FromArgb(31, 72, 161);
            this.BorderColor2 = Color.FromArgb(68, 135, 228);

            // Button Text Color

            this.TextColor = Color.White;
            this.SelectedTextColor = Color.FromArgb(30, 57, 91);
            this.HoverTextColor = Color.FromArgb(30, 57, 91);

            // Button normal color

            this.ButtonNormalColor1 = Color.FromArgb(42, 126, 43);
            this.ButtonNormalColor2 = Color.FromArgb(94, 184, 67);
            this.ButtonNormalColor3 = Color.FromArgb(42, 126, 43);
            this.ButtonNormalColor4 = Color.FromArgb(94, 184, 67);

            // Button mouseover color

            this.ButtonMouseOverColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonMouseOverColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonMouseOverColor3 = Color.FromArgb(249, 225, 137);
            this.ButtonMouseOverColor4 = Color.FromArgb(251, 249, 224);

            // Button selected color

            this.ButtonSelectedColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonSelectedColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonSelectedColor3 = Color.FromArgb(255, 229, 117);
            this.ButtonSelectedColor4 = Color.FromArgb(255, 216, 107);
        }

        public override string ToString()
        {
            return "Office2010Green";
        }
    }

    #endregion Office 2010 GREEN

    #region Office 2010 Red

    public class Office2010Red : Colortable
    {
        public Office2010Red()
        {
            // Border Color

            this.BorderColor1 = Color.FromArgb(31, 72, 161);
            this.BorderColor2 = Color.FromArgb(68, 135, 228);

            // Button Text Color

            this.TextColor = Color.White;
            this.SelectedTextColor = Color.FromArgb(30, 57, 91);
            this.HoverTextColor = Color.FromArgb(30, 57, 91);

            // Button normal color

            this.ButtonNormalColor1 = Color.FromArgb(227, 77, 45);
            this.ButtonNormalColor2 = Color.FromArgb(245, 148, 64);
            this.ButtonNormalColor3 = Color.FromArgb(227, 77, 45);
            this.ButtonNormalColor4 = Color.FromArgb(245, 148, 64);

            // Button mouseover color

            this.ButtonMouseOverColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonMouseOverColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonMouseOverColor3 = Color.FromArgb(249, 225, 137);
            this.ButtonMouseOverColor4 = Color.FromArgb(251, 249, 224);

            // Button selected color

            this.ButtonSelectedColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonSelectedColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonSelectedColor3 = Color.FromArgb(255, 229, 117);
            this.ButtonSelectedColor4 = Color.FromArgb(255, 216, 107);
        }

        public override string ToString()
        {
            return "Office2010Red";
        }
    }

    #endregion Office 2010 Red

    #region Office 2010 Pink

    public class Office2010Pink : Colortable
    {
        public Office2010Pink()
        {
            // Border Color

            this.BorderColor1 = Color.FromArgb(31, 72, 161);
            this.BorderColor2 = Color.FromArgb(68, 135, 228);

            // Button Text Color

            this.TextColor = Color.White;
            this.SelectedTextColor = Color.FromArgb(30, 57, 91);
            this.HoverTextColor = Color.FromArgb(30, 57, 91);

            // Button normal color

            this.ButtonNormalColor1 = Color.FromArgb(175, 6, 77);
            this.ButtonNormalColor2 = Color.FromArgb(222, 52, 119);
            this.ButtonNormalColor3 = Color.FromArgb(175, 6, 77);
            this.ButtonNormalColor4 = Color.FromArgb(222, 52, 119);

            // Button mouseover color

            this.ButtonMouseOverColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonMouseOverColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonMouseOverColor3 = Color.FromArgb(249, 225, 137);
            this.ButtonMouseOverColor4 = Color.FromArgb(251, 249, 224);

            // Button selected color

            this.ButtonSelectedColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonSelectedColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonSelectedColor3 = Color.FromArgb(255, 229, 117);
            this.ButtonSelectedColor4 = Color.FromArgb(255, 216, 107);
        }

        public override string ToString()
        {
            return "Office2010Pink";
        }
    }

    #endregion Office 2010 Pink

    #region Office 2010 White

    public class Office2010White : Colortable
    {
        public Office2010White()
        {
            // Border Color

            this.BorderColor1 = Color.FromArgb(31, 72, 161);
            this.BorderColor2 = Color.FromArgb(68, 135, 228);

            // Button Text Color

            this.TextColor = Color.Black;
            this.SelectedTextColor = Color.FromArgb(30, 57, 91);
            this.HoverTextColor = Color.FromArgb(30, 57, 91);

            // Button normal color

            this.ButtonNormalColor1 = Color.FromArgb(154, 154, 154);
            this.ButtonNormalColor2 = Color.FromArgb(255, 255, 255);
            this.ButtonNormalColor3 = Color.FromArgb(235, 235, 235);
            this.ButtonNormalColor4 = Color.FromArgb(255, 255, 255);

            // Button mouseover color

            this.ButtonMouseOverColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonMouseOverColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonMouseOverColor3 = Color.FromArgb(249, 225, 137);
            this.ButtonMouseOverColor4 = Color.FromArgb(251, 249, 224);

            // Button selected color

            this.ButtonSelectedColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonSelectedColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonSelectedColor3 = Color.FromArgb(255, 229, 117);
            this.ButtonSelectedColor4 = Color.FromArgb(255, 216, 107);
        }

        public override string ToString()
        {
            return "Office2010White";
        }
    }

    #endregion Office 2010 White

    #region Office 2010 Yellow

    public class Office2010Yellow : Colortable
    {
        public Office2010Yellow()
        {
            // Border Color

            this.BorderColor1 = Color.FromArgb(31, 72, 161);
            this.BorderColor2 = Color.FromArgb(68, 135, 228);

            // Button Text Color

            this.TextColor = Color.White;
            this.SelectedTextColor = Color.FromArgb(30, 57, 91);
            this.HoverTextColor = Color.FromArgb(30, 57, 91);

            // Button normal color

            this.ButtonNormalColor1 = Color.FromArgb(252, 161, 8);
            this.ButtonNormalColor2 = Color.FromArgb(251, 191, 45);
            this.ButtonNormalColor3 = Color.FromArgb(252, 161, 8);
            this.ButtonNormalColor4 = Color.FromArgb(251, 191, 45);

            // Button mouseover color

            this.ButtonMouseOverColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonMouseOverColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonMouseOverColor3 = Color.FromArgb(249, 225, 137);
            this.ButtonMouseOverColor4 = Color.FromArgb(251, 249, 224);

            // Button selected color

            this.ButtonSelectedColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonSelectedColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonSelectedColor3 = Color.FromArgb(255, 229, 117);
            this.ButtonSelectedColor4 = Color.FromArgb(255, 216, 107);
        }

        public override string ToString()
        {
            return "Office2010Yellow";
        }
    }

    #endregion Office 2010 Yellow

    #region Office 2010 Publisher

    public class Office2010Publisher : Colortable
    {
        public Office2010Publisher()
        {
            // Border Color

            this.BorderColor1 = Color.FromArgb(31, 72, 161);
            this.BorderColor2 = Color.FromArgb(68, 135, 228);

            // Button Text Color

            this.TextColor = Color.White;
            this.SelectedTextColor = Color.FromArgb(30, 57, 91);
            this.HoverTextColor = Color.FromArgb(30, 57, 91);

            // Button normal color

            this.ButtonNormalColor1 = Color.FromArgb(0, 126, 126);
            this.ButtonNormalColor2 = Color.FromArgb(31, 173, 167);
            this.ButtonNormalColor3 = Color.FromArgb(0, 126, 126);
            this.ButtonNormalColor4 = Color.FromArgb(31, 173, 167);

            // Button mouseover color

            this.ButtonMouseOverColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonMouseOverColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonMouseOverColor3 = Color.FromArgb(249, 225, 137);
            this.ButtonMouseOverColor4 = Color.FromArgb(251, 249, 224);

            // Button selected color

            this.ButtonSelectedColor1 = Color.FromArgb(236, 199, 87);
            this.ButtonSelectedColor2 = Color.FromArgb(252, 243, 215);
            this.ButtonSelectedColor3 = Color.FromArgb(255, 229, 117);
            this.ButtonSelectedColor4 = Color.FromArgb(255, 216, 107);
        }

        public override string ToString()
        {
            return "Office2010Publisher";
        }
    }

    #endregion Office 2010 Publisher

    public partial class XButton : Button
    {
        #region Fields

        private Theme thm = Theme.MSOffice2010_BLUE;

        private enum MouseState
        { None = 1, Down = 2, Up = 3, Enter = 4, Leave = 5, Move = 6 }

        private MouseState MState = MouseState.None;

        #endregion Fields

        #region Constructor

        public XButton()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor |
                      ControlStyles.Opaque |
                      ControlStyles.ResizeRedraw |
                      ControlStyles.OptimizedDoubleBuffer |
                      ControlStyles.CacheText, // We gain about 2% in painting by avoiding extra GetWindowText calls
                      true);

            this.colorTable = new Colortable();

            this.MouseLeave += new EventHandler(_MouseLeave);
            this.MouseDown += new MouseEventHandler(_MouseDown);
            this.MouseUp += new MouseEventHandler(_MouseUp);
            this.MouseMove += new MouseEventHandler(_MouseMove);
        }

        #endregion Constructor

        #region Events

        #region Paint Transparent Background

        protected void PaintTransparentBackground(Graphics g, Rectangle clipRect)
        {
            // check if we have a parent
            if (this.Parent != null)
            {
                // convert the clipRects coordinates from ours to our parents
                clipRect.Offset(this.Location);

                PaintEventArgs e = new PaintEventArgs(g, clipRect);

                // save the graphics state so that if anything goes wrong
                // we're not fubar
                GraphicsState state = g.Save();

                try
                {
                    // move the graphics object so that we are drawing in
                    // the correct place
                    g.TranslateTransform((float)-this.Location.X, (float)-this.Location.Y);

                    // draw the parents background and foreground
                    this.InvokePaintBackground(this.Parent, e);
                    this.InvokePaint(this.Parent, e);

                    return;
                }
                finally
                {
                    // reset everything back to where they were before
                    g.Restore(state);
                    clipRect.Offset(-this.Location.X, -this.Location.Y);
                }
            }

            // we don't have a parent, so fill the rect with
            // the default control color
            g.FillRectangle(SystemBrushes.Control, clipRect);
        }

        #endregion Paint Transparent Background

        #region Mouse Events

        private void _MouseDown(object sender, MouseEventArgs mevent)
        {
            MState = MouseState.Down;
            Invalidate();
        }

        private void _MouseUp(object sender, MouseEventArgs mevent)
        {
            MState = MouseState.Up;
            Invalidate();
        }

        private void _MouseMove(object sender, MouseEventArgs mevent)
        {
            MState = MouseState.Move;
            Invalidate();
        }

        private void _MouseLeave(object sender, EventArgs e)
        {
            MState = MouseState.Leave;
            Invalidate();
        }

        #endregion Mouse Events

        #region Path

        public static GraphicsPath GetRoundedRect(RectangleF r, float radius)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.StartFigure();
            r = new RectangleF(r.Left, r.Top, r.Width, r.Height);
            if (radius <= 0.0F || radius <= 0.0F)
            {
                gp.AddRectangle(r);
            }
            else
            {
                gp.AddArc((float)r.X, (float)r.Y, radius, radius, 180, 90);
                gp.AddArc((float)r.Right - radius, (float)r.Y, radius - 1, radius, 270, 90);
                gp.AddArc((float)r.Right - radius, ((float)r.Bottom) - radius - 1, radius - 1, radius, 0, 90);
                gp.AddArc((float)r.X, ((float)r.Bottom) - radius - 1, radius - 1, radius, 90, 90);
            }
            gp.CloseFigure();
            return gp;
        }

        #endregion Path

        protected override void OnPaint(PaintEventArgs e)
        {
            this.PaintTransparentBackground(e.Graphics, this.ClientRectangle);

            #region Painting

            //now let's we begin painting
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            #endregion Painting

            #region Color

            Color tTopColorBegin = this.colorTable.ButtonNormalColor1;
            Color tTopColorEnd = this.colorTable.ButtonNormalColor2;
            Color tBottomColorBegin = this.colorTable.ButtonNormalColor3;
            Color tBottomColorEnd = this.colorTable.ButtonNormalColor4;
            Color Textcol = this.colorTable.TextColor;

            if (!this.Enabled)
            {
                tTopColorBegin = Color.FromArgb((int)(tTopColorBegin.GetBrightness() * 255),
                    (int)(tTopColorBegin.GetBrightness() * 255),
                    (int)(tTopColorBegin.GetBrightness() * 255));
                tBottomColorEnd = Color.FromArgb((int)(tBottomColorEnd.GetBrightness() * 255),
                    (int)(tBottomColorEnd.GetBrightness() * 255),
                    (int)(tBottomColorEnd.GetBrightness() * 255));
            }
            else
            {
                if (MState == MouseState.None || MState == MouseState.Leave)
                {
                    tTopColorBegin = this.colorTable.ButtonNormalColor1;
                    tTopColorEnd = this.colorTable.ButtonNormalColor2;
                    tBottomColorBegin = this.colorTable.ButtonNormalColor3;
                    tBottomColorEnd = this.colorTable.ButtonNormalColor4;
                    Textcol = this.colorTable.TextColor;
                }
                else if (MState == MouseState.Down)
                {
                    tTopColorBegin = this.colorTable.ButtonSelectedColor1;
                    tTopColorEnd = this.colorTable.ButtonSelectedColor2;
                    tBottomColorBegin = this.colorTable.ButtonSelectedColor3;
                    tBottomColorEnd = this.colorTable.ButtonSelectedColor4;
                    Textcol = this.colorTable.SelectedTextColor;
                }
                else if (MState == MouseState.Move || MState == MouseState.Up)
                {
                    tTopColorBegin = this.colorTable.ButtonMouseOverColor1;
                    tTopColorEnd = this.colorTable.ButtonMouseOverColor2;
                    tBottomColorBegin = this.colorTable.ButtonMouseOverColor3;
                    tBottomColorEnd = this.colorTable.ButtonMouseOverColor4;
                    Textcol = this.colorTable.HoverTextColor;
                }
            }

            #endregion Color

            #region Theme 2010

            if (thm == Theme.MSOffice2010_BLUE || thm == Theme.MSOffice2010_Green || thm == Theme.MSOffice2010_Yellow || thm == Theme.MSOffice2010_Publisher ||
                thm == Theme.MSOffice2010_RED || thm == Theme.MSOffice2010_WHITE || thm == Theme.MSOffice2010_Pink)
            {
                Paint2010Background(e, g, tTopColorBegin, tTopColorEnd, tBottomColorBegin, tBottomColorEnd);
                TEXTandIMAGE(this.ClientRectangle, g, Textcol);
            }

            #endregion Theme 2010
        }

        #region Paint 2010 Background

        protected void Paint2010Background(PaintEventArgs e, Graphics g, Color tTopColorBegin, Color tTopColorEnd, Color tBottomColorBegin, Color tBottomColorEnd)
        {
            int _roundedRadiusX = 6;

            Rectangle r = new Rectangle(this.ClientRectangle.Left, this.ClientRectangle.Top, this.ClientRectangle.Width, this.ClientRectangle.Height);
            Rectangle j = r;
            Rectangle r2 = r;
            r2.Inflate(-1, -1);
            Rectangle r3 = r2;
            r3.Inflate(-1, -1);

            //rectangle for gradient, half upper and lower
            RectangleF halfup = new RectangleF(r.Left, r.Top, r.Width, r.Height);
            RectangleF halfdown = new RectangleF(r.Left, r.Top + (r.Height / 2) - 1, r.Width, r.Height);

            //BEGIN PAINT BACKGROUND
            //for half upper, we paint using linear gradient
            using (GraphicsPath thePath = GetRoundedRect(r, _roundedRadiusX))
            {
                LinearGradientBrush lgb = new LinearGradientBrush(halfup, tBottomColorEnd, tBottomColorBegin, 90f, true);

                Blend blend = new Blend(4);
                blend.Positions = new float[] { 0, 0.18f, 0.35f, 1f };
                blend.Factors = new float[] { 0f, .4f, .9f, 1f };
                lgb.Blend = blend;
                g.FillPath(lgb, thePath);
                lgb.Dispose();

                //for half lower, we paint using radial gradient
                using (GraphicsPath p = new GraphicsPath())
                {
                    p.AddEllipse(halfdown); //make it radial
                    using (PathGradientBrush gradient = new PathGradientBrush(p))
                    {
                        gradient.WrapMode = WrapMode.Clamp;
                        gradient.CenterPoint = new PointF(Convert.ToSingle(halfdown.Left + halfdown.Width / 2), Convert.ToSingle(halfdown.Bottom));
                        gradient.CenterColor = tBottomColorEnd;
                        gradient.SurroundColors = new Color[] { tBottomColorBegin };

                        blend = new Blend(4);
                        blend.Positions = new float[] { 0, 0.15f, 0.4f, 1f };
                        blend.Factors = new float[] { 0f, .3f, 1f, 1f };
                        gradient.Blend = blend;

                        g.FillPath(gradient, thePath);
                    }
                }
                //END PAINT BACKGROUND

                //BEGIN PAINT BORDERS
                using (GraphicsPath gborderDark = thePath)
                {
                    using (Pen p = new Pen(tTopColorBegin, 1))
                    {
                        g.DrawPath(p, gborderDark);
                    }
                }
                using (GraphicsPath gborderLight = GetRoundedRect(r2, _roundedRadiusX))
                {
                    using (Pen p = new Pen(tTopColorEnd, 1))
                    {
                        g.DrawPath(p, gborderLight);
                    }
                }
                using (GraphicsPath gborderMed = GetRoundedRect(r3, _roundedRadiusX))
                {
                    SolidBrush bordermed = new SolidBrush(Color.FromArgb(50, tTopColorEnd));
                    using (Pen p = new Pen(bordermed, 1))
                    {
                        g.DrawPath(p, gborderMed);
                    }
                }
                //END PAINT BORDERS
            }
        }

        #endregion Paint 2010 Background

        #region Paint TEXT AND IMAGE

        protected void TEXTandIMAGE(Rectangle Rec, Graphics g, Color textColor)
        {
            //BEGIN PAINT TEXT
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            #region Top

            if (this.TextAlign == ContentAlignment.TopLeft)
            {
                sf.LineAlignment = StringAlignment.Near;
                sf.Alignment = StringAlignment.Near;
            }
            else if (this.TextAlign == ContentAlignment.TopCenter)
            {
                sf.LineAlignment = StringAlignment.Near;
                sf.Alignment = StringAlignment.Center;
            }
            else if (this.TextAlign == ContentAlignment.TopRight)
            {
                sf.LineAlignment = StringAlignment.Near;
                sf.Alignment = StringAlignment.Far;
            }

            #endregion Top

            #region Middle

            else if (this.TextAlign == ContentAlignment.MiddleLeft)
            {
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Near;
            }
            else if (this.TextAlign == ContentAlignment.MiddleCenter)
            {
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
            }
            else if (this.TextAlign == ContentAlignment.MiddleRight)
            {
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Far;
            }

            #endregion Middle

            #region Bottom

            else if (this.TextAlign == ContentAlignment.BottomLeft)
            {
                sf.LineAlignment = StringAlignment.Far;
                sf.Alignment = StringAlignment.Near;
            }
            else if (this.TextAlign == ContentAlignment.BottomCenter)
            {
                sf.LineAlignment = StringAlignment.Far;
                sf.Alignment = StringAlignment.Center;
            }
            else if (this.TextAlign == ContentAlignment.BottomRight)
            {
                sf.LineAlignment = StringAlignment.Far;
                sf.Alignment = StringAlignment.Far;
            }

            #endregion Bottom

            if (this.ShowKeyboardCues)
                sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
            else
                sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
            g.DrawString(this.Text, this.Font, new SolidBrush(textColor), Rec, sf);
        }

        #endregion Paint TEXT AND IMAGE

        #endregion Events

        #region Properties

        #region ColorTable

        private Colortable colorTable = null;

        [DefaultValue(typeof(Colortable), "Office2010Blue")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Colortable ColorTable
        {
            get
            {
                if (colorTable == null)
                    colorTable = new Colortable();

                return colorTable;
            }
            set
            {
                if (value == null)
                    value = Colortable.Office2010Blue;

                colorTable = (Colortable)value;

                this.Invalidate();
            }
        }

        public Theme Theme
        {
            get
            {
                if (this.colorTable == Colortable.Office2010Green)
                {
                    return Theme.MSOffice2010_Green;
                }
                else if (this.colorTable == Colortable.Office2010Red)
                {
                    return Theme.MSOffice2010_RED;
                }
                else if (this.colorTable == Colortable.Office2010Pink)
                {
                    return Theme.MSOffice2010_Pink;
                }
                else if (this.colorTable == Colortable.Office2010Yellow)
                {
                    return Theme.MSOffice2010_Yellow;
                }
                else if (this.colorTable == Colortable.Office2010White)
                {
                    return Theme.MSOffice2010_WHITE;
                }
                else if (this.colorTable == Colortable.Office2010Publisher)
                {
                    return Theme.MSOffice2010_Publisher;
                }

                return Theme.MSOffice2010_BLUE;
            }

            set
            {
                this.thm = value;

                if (thm == Theme.MSOffice2010_Green)
                {
                    this.colorTable = Colortable.Office2010Green;
                }
                else if (thm == Theme.MSOffice2010_RED)
                {
                    this.colorTable = Colortable.Office2010Red;
                }
                else if (thm == Theme.MSOffice2010_Pink)
                {
                    this.colorTable = Colortable.Office2010Pink;
                }
                else if (thm == Theme.MSOffice2010_WHITE)
                {
                    this.colorTable = Colortable.Office2010White;
                }
                else if (thm == Theme.MSOffice2010_Yellow)
                {
                    this.colorTable = Colortable.Office2010Yellow;
                }
                else if (thm == Theme.MSOffice2010_Publisher)
                {
                    this.colorTable = Colortable.Office2010Publisher;
                }
                else
                {
                    this.colorTable = Colortable.Office2010Blue;
                }
            }
        }

        #endregion ColorTable

        #region Background Image

        [Browsable(false)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        #endregion Background Image

        #endregion Properties
    }
}