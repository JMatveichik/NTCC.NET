   M S B A M L   `   `   `  ����O  KConfigurationExample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullO,clr-namespace:ConfigurationExample.UtilitiesConfigurationExample.Utilities  5   	   %    ConfigurationExample.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   B	utilities,clr-namespace:ConfigurationExample.Utilities     5   	     � Title$  
MainWindow��5   	   $	��700��6   $
��1024��6)   .��5      �  � 	Resources 5   
   9   3ConfigurationExample.Utilities.EnumBooleanConverter   EnumBooleanConverter��%   &	          5      6-   5	       � RowDefinitions 5
   
   �� 5      $��* �6   �� 5      $��40 �6   �� 5      $��40 �6   �� 5      $��40 �6   �� 5      $��40 �6   .+�5   
   �  � ShowGridLines .  � ColumnDefinitions 5      �� 5      �� 5       5      �� 5      �� 5      .+�5      �� -   5         ��Name$ TrackingElement��5      
���  ��$��2��6N   $��2��6b   $	��500��6s   $	��500��6�   ��61   Z� 6�   5      � 5   
   $��1��6   $��Stretch=�6    5       �� 5!      $
��Auto �6#   �� 5"      .+�5$      �� $��Notification text:��6   $��Center=�62   $��5q�6M   $
��Bold"�6X   �� 5%      -   5         ��Name$ SampleTextInput��5%   &   $��1��6   $��Notification��6?   $��5q�6S   $��Center=�6_   $��Stretch=�6z   $��20��6�   5&      � 5(   
   $��2��6    5)      �� 5*      �� 5+      �� 5,      .+�5.      �� $��0��6    �� Orientation$ 
Horizontal=�6)   $��Center=�6C   .+�5/      �� $��Position provider:��6   $��Center=�66   $��5q�6Q   $
��Bold"�6\   	� 50      $��Center=�6    d� 	IsChecked 69   �� 	 �� Path$	 PositionProviderType�
 �� Mode$
 TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� 	Converter# [   �� ConverterParameter$ Window��.��6�   Window6�   	� 51      $��Center=�6   $
��10 0q�6�    69   �� $	 PositionProviderType�$
 TwoWay=�$ PropertyChanged=�# [  $ Screen��.��6�   Main screen6  	� 52      $��Center=�6    69   �� $	 PositionProviderType�$
 TwoWay=�$ PropertyChanged=�# [  $ Control��.��6�   	Control6�   53      ]� 54      $��1��6   $��2��6(   .+�55      �� $��Corner:��6   $��Center=�6+   $��5q�6F   $
��Bold"�6Q   	� 56      $��Center=�6    69   �� $	 Corner�$
 TwoWay=�$ PropertyChanged=�# [  $ TopLeft��.��6�   	TopLeft6�   	� 57      $��Center=�6   $
��20 0q�6�    69   �� $	 Corner�$
 TwoWay=�$ PropertyChanged=�# [  $ 
BottomLeft��.��6�   
BottomLeft6�   	� 58      $��Center=�6    69   �� $	 Corner�$
 TwoWay=�$ PropertyChanged=�# [  $ TopRight��.��6�   
TopRight6�   	� 59      $��Center=�6   $
��20 0q�6�    69   �� $	 Corner�$
 TwoWay=�$ PropertyChanged=�# [  $ BottomRight��.��6�   BottomRight6�   	� 5:      $��Center=�6    69   �� $	 Corner�$
 TwoWay=�$ PropertyChanged=�# [  $ BottomCenter��.��6�   BottomCenter6�   5;      5<      � 5>   
   $��3��6    5?      �� 5@      $
��Auto �6#   �� 5A      �� 5B      .+�5D      �� $��0��6   $ 
Horizontal=�6)   $��Center=�6C   .+�5E      �� $��	Lifetime:��6   $��Center=�6-   $��5q�6H   $
��Bold"�6S   	� 5F      $��Center=�6    69   �� $	 Lifetime�$
 TwoWay=�$ PropertyChanged=�# [  $ Basic��.��6�   Basic6�   	� 5G      $��Center=�6    69   �� $	 Lifetime�$
 TwoWay=�$ PropertyChanged=�# [  $ 	TimeBased��.��6�   
Time based6�   5H      �� 5I      $��2��6   $ 
Horizontal=�6)   $��Center=�6C   .+�5J      �� $��Options:��6   $��Center=�6,   $��5q�6G   $
��Bold"�6R   �� 5K      $��Center=�6N    6   �� *ShowCloseButton+$
 TwoWay=�.��6i   ShowCloseButton6z   �� 5L      $��Center=�6Q   $��20 0 0 0q�6l    6   �� *FreezeOnMouseEnter+$
 TwoWay=�.��6~   FreezeOnMouseEnter6�   �� 5M      $��Center=�6F   $��20 0 0 0q�6a    6   �� *	TopMost+$
 TwoWay=�.��6s   	Topmost6|   5N      5O      � 5R   
   $��4��6    5S      �� 5T      $
��Auto �6#   �� 5U      .+�5W      �� $��Notification type:��6   $��Center=�62   $��5q�6M   $
��Bold"�6X   �� 5X      $ 
Horizontal=�6   $
��Left=�62   $��1��6M   .+�5Y      �� -   5        $��Information��5Y      $��5q�6/   $	��100��6:   6F   �� 5Z      -   5        $��Success��5Z      $��5q�6+   $	��100��66   6B   �� 5[      -   5        $��Warning��5[      $��5q�6+   $	��100��66   6B   �� 5\      -   5        $��Error��5\      $��5q�6)   $	��100��64   6@   �� 5]      -   5        $��
Customized��5]      $��5q�6.   $	��100��69   6E   �� 5_      -   5        $��
Clear last��5_      $��5q�6.   $	��100��69   6E   �� 5`      -	   5        $��	Clear all��5`      $��5q�6-   $	��100��68   6D   �� 5a      -
   5        $��Clear by Tag��5a      $��5q�60   $	��100��6;   6G   �� 5b      -   5        $��Clear by Message��5b      $��5q�64   $	��100��6?   6K   5d      5e      5g      5h      