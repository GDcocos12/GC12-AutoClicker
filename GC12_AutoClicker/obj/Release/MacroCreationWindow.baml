   M S B A M L   `   `   `  ���� K  GGC12_AutoClicker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:GC12_AutoClickerGC12_AutoClicker  *    $GC12_AutoClicker.MacroCreationWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     0localclr-namespace:GC12_AutoClicker       � 
ResizeMode$  NoResize=� � Title$ Create Macro��$	��450��$	��540��.��� $��10q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � � ColumnDefinitions �� $
��Auto ��� $��* �.+��� $��0��$��0��$��Macro Name:���� -    ��Name$ MacroNameTextBox��$��0��$��1��$��5q��� $��1��$��0��$��Actions:���� -    ��Name$ ActionsListView��$��2��$��0��$��2��$��5q� �� View �� .P��� $��X�� �� Width$ 50�� �� DisplayMemberBinding �� *X+�� $��Y��$ 50�� �� *Y+�� $��Duration (ms)��$ 90�� �� *ClickDuration+�� $��
Delay (ms)��$ 90�� �� *Delay+�� $��Repeats��$ 70�� �� *Repetitions+�� $��3��$��0��$��2��	 �� Orientation$	 
Horizontal=�$��5q�.+��� $��X:��$��Center=��� -   $ XTextBox��$��50��$��5,0,5,0q��� $��Y:��$��Center=��� -   $ YTextBox��$��50��$��5,0,5,0q��� $��	Duration:��$��Center=��� -   $ DurationTextBox��$��50��$��5,0,5,0q��� $��Delay:��$��Center=��� -   $ DelayTextBox��$��50��$��5,0,5,0q��� $��Repeats:��$��Center=��� -   $ RepetitionsTextBox��$��50��$��5,0,5,0q��� -   $��4��$��0��$��2��$��Add Action (F9)��$��5q��� $��5��$��0��$��2��$	 
Horizontal=�$��Center=�$��0,5,0,5q�.+��� -	   
 ��Name$
 MoveUpButton��$��Move Up��$��5q�$��5,2,5,2q�{�.  �� -
   $
 MoveDownButton��$��	Move Down��$��5q�$��5,2,5,2q�{�.  �� -   $
 DeleteActionButton��$��Delete Action��$��5q�$��5,2,5,2q�{�.  �� $��6��$��0��$��2��$	 
Horizontal=�$��Right=�$��0,10,0,0q�.+��� -   $
 
SaveButton��$
��Save��$��5q�$��	10,2,10,2q��� -   $
 CancelButton��$��Cancel��$��5q�$��	10,2,10,2q��� -   $
 InstructionsButton��$��Instructions��$��5q�$��5,2,5,2q�