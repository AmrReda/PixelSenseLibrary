using System.Windows.Controls;

namespace PixelSenseLibrary.Extensions
{
    public static class ListBoxExtensions
    {
        public static void SelectFirst(this ListBox refListBox)
        {
            refListBox.SelectedIndex = 0;
            refListBox.ScrollIntoView(refListBox.SelectedItem);
        }

        public static void SelectLast(this ListBox refListBox)
        {
            refListBox.SelectedIndex = refListBox.Items.Count - 1;
            refListBox.ScrollIntoView(refListBox.SelectedItem);
        }

        public static void SelectPrevious(this ListBox refListBox, bool bWrapValue = false)
        {
            if (refListBox.SelectedIndex > 0)
            {
                ListBox listBox = refListBox;
                int num = listBox.SelectedIndex - 1;
                listBox.SelectedIndex = num;
            }
            else if (refListBox.SelectedIndex == 0 && bWrapValue)
                refListBox.SelectedIndex = refListBox.Items.Count - 1;
            refListBox.ScrollIntoView(refListBox.SelectedItem);
        }

        public static void SelectNext(this ListBox refListBox, bool bWrapValue = false)
        {
            int num1 = refListBox.Items.Count - 1;
            if (refListBox.SelectedIndex < num1)
            {
                ListBox listBox = refListBox;
                int num2 = listBox.SelectedIndex + 1;
                listBox.SelectedIndex = num2;
            }
            else if (refListBox.SelectedIndex == num1 && bWrapValue)
                refListBox.SelectedIndex = 0;
            refListBox.ScrollIntoView(refListBox.SelectedItem);
        }
    }
}