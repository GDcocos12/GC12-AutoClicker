   M S B A M L   `   `   `  ���� K  GGC12_AutoClicker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:GC12_AutoClickerGC12_AutoClicker  !    GC12_AutoClicker.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     0localclr-namespace:GC12_AutoClicker       � 
ResizeMode$  NoResize=� � Title$ GC12 | AutoClicker��$	��350��$	��585��.��� $��10q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto � � ColumnDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��0��$��3��
�������$��1q� �� CornerRadius$ 8�����#EEEEEE �� Padding$ 5q�._��  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � �� $
��Auto ��� $��* ��� $
��Auto �.+��� $��0��$��0��$��Interval (ms):���� -    ��Name$ IntervalTextBox��$��0��$��1��$��5q��� $��1��$��0��$��	Position:���� $��1��$��1�� �� Orientation$ 
Horizontal=�$��5q�.+�	� -    	�Name$  CurrentPositionRadioButton��$��Current��	 	� 	GroupName$	 PositionGroup��$��0,0,10,0q�	� -   $! SpecificPositionRadioButton��$��Specific��$	 PositionGroup��$��0,0,10,0q�	� -   $ MacroRadioButton��$��Macro��$	 PositionGroup���� $��2��$��0��$��X:��$��Center=��� -   $ XPosTextBox��$��2��$��1��$��5q�{�.  �� $��3��$��0��$��Y:��$��Center=��� -   $ YPosTextBox��$��3��$��1��$��5q�{�.  �� $��4��$��0��$��Clicks:���� -   $ ClicksTextBox��$��4��$��1��$��5q��� $��5��$��0��$��Click Type:���� -   
 ��Name$
 ClickTypeComboBox��$��5��$��1��$��5q��� $��6��$��0��$��Hotkey:��$��Center=�$��10(�$��0,2,0,2q��� -	   $ HotkeyTextBox��$��6��$��1�� �� 
IsReadOnly . $��5,2,5,2q�$��10(�$��Center=��� -
    ��Name$ CursorPositionLabel��$��6��$��2��$��5,2,5,2q�$��
X: 0, Y: 0��$��Center=�$��10(��� $��7��$��0��$��Macro:��$��Center=��� -   $
 MacroComboBox��$��7��$��1��$��5q� �� DisplayMemberPath$
 Name���� $��7��$��2��$ 
Horizontal=�.+��� -    ��Name$ CreateMacroButton��$��Create��$��5,5,0,5q��� -   $ LoadMacroButton��$
��Load��$��5,5,0,5q��� -   $ ExportMacroButton��$��Export��$��5,5,0,5q��� -   $ EditMacroButton��$
��Edit��$��5,5,0,5q��� $��2��$��3��$ 
Horizontal=�$��Center=�$��0,10,0,0q�.+��� -   $ StartButton��$��Start��$��5q�$��	10,2,10,2q��� -   $ 
StopButton��$
��Stop��$��5q�$��	10,2,10,2q��� -   $ HotkeyButton��$��Enable Hotkey��$��5q�$��5,2,5,2q��� -   $ InstructionsButton��$��Instructions��$��5q�$��5,2,5,2q�