   M S B A M L   `   `   `  ����>  :RPR, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:RPRRPR  5   	       RPR.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:RPR     5   	   -   5          � Title$	  RPR��5   	   $
��1080��6   $
��1920��6#   $
��1200��6d   $	��750��6t    � WindowState$ 	Maximized=�6�   .��5	      � .+�5
   
   �� -   5         ��Name$ MainView��5
      
���   � K� ZIndex$	 500��6�   5      �� 5   
   -   5        $ 	StartMenu��5      $
��1064��6$   $
��1920��62   $
 1200��6?   .+�5      � $
��1064��6   $
��1920��6!   
������� � RowDefinitions 5      �� 5      6&   �� 5      6&   �� 5      6&   .+�5      �� $��0��6   $��0��6%   $��RPRLab��65   $��Center=�6F   $��Center=�6a   $
��Bold"�6~   $��36(�6�   $��Comic Sans MS)�6�   6�   9� 5      $��1��6   $��0��6(   $&�� Создать новый мир��68   $	��Top=�6S   $��Center=�6k   $��Comic Sans MS)�6�   $��20(�6�   .��5      �� .+�5      ��  �� Orientation$ 
Horizontal=�6%   .+�5      �� $ ��Название мира:��6$   $��Comic Sans MS)�6=   $��16(�6X   6h   �� 5      -   5         ��Name$ 	WorldName��5   &   $	��205��69   $��Comic Sans MS)�6E   $��16(�6`   6q   5      �� 5      �� 5      $ 
Horizontal=�6%   .+�5      �� $ ��Описание мира:��6$   $��Comic Sans MS)�6=   $��16(�6X   6h   �� 5      -   5        $ DescriptionWorld��5   &   $	��199��6@   $
��60px��6L   $��Comic Sans MS)�6Z   $��16(�6u   " �� VerticalScrollBarVisibility$
 Auto=�6�   6�   5      �� 5       �� 5!      $ 
Horizontal=�6%   .+�5"      �� -   5         ��Name$ CreateButton��5"   %   $��Создать��6;   $	��335��6h   $��Comic Sans MS)�6t   $��16(�6�   5#      5$      5%      9� 5&      $��2��6   $��0��6(   $6��0Запустить сохранённый мир��68   $	��Top=�6[   $��Center=�6s   $��Comic Sans MS)�6�   $��20(�6�   .��5'      �� .+�5(      �� $ 
Horizontal=�6%   .+�5)      �� $ ��Название мира:��6$   $��Comic Sans MS)�6=   $��16(�6X   6h   �� 5*      -   5        $ SavedWorldName��5*   &   $��200px��6>   $��Comic Sans MS)�6L   $��16(�6g   6w   5+      �� 5,      �� 5-      $ 
Horizontal=�6%   .+�5.      �� -   5        $ 
OpenButton��5.   %   $��Открыть��69   $	��330��6d   $��Comic Sans MS)�6p   $��16(�6�   5/      50      51      52      53      � 54   
   $
 1000��6   .+�55      �� -	   5        $ 
MenuButton��55         DefaultButtonStyle#���   6)   $��>��6V   $��0,127,355,0q�6e   $��Right=�6z   $��20��6�   $��70��6�   $	��Top=�6�   ���	#50FFFFFF
����������	#50FFFFFF6  �� 56      -
   5        	 ��Name$	 	InnerMenu��56      $��0,0,0,1q�6)   $��Right=�6:   $	��353��6V   .@�57      9� 
 9� ExpandDirection$

 Down=�6   $!��Базовые фигуры��62   .��58      �� .+�59      �� ���	#FF7B7676$	��345��6<   $ 
Horizontal=�6H   .+�5:      A� $��45��6&   $��45��61   ���	#FFF50000$��Center=�6N   $
��Left=�6i   6�   �� 5;      -   5        $ 	MenuItem1��5;   %   #���   68   $��Ellipse��6e   ���	#FFF9F6F6$��Center=�6�   ���	#00DDDDDD$	��296��6�   $��39��6�   ���	#00DDDDDD
 � Tag$ Ellipse��6�   6�   5<      �� 5=      �� 5>      ���	#FF7B7676$	��345��6<   $ 
Horizontal=�6H   .+�5?      �� $��45��6(   $��45��63   ���	#FFF50000$��Center=�6P   $
��Left=�6k   6�   �� 5@      -   5        $ 	MenuItem2��5@   %   #���   68   $��	Rectangle��6e   ���	#FFF9F6F6$��Center=�6�   ���	#00DDDDDD$	��296��6�   $��37��6�   ���	#00DDDDDD$ 	Rectangle��6�   6  5A      �� 5B      �� 5C      ���	#FF7B7676$	��345��6<   $ 
Horizontal=�6H   .+�5D      �� 	 �� X1$ 0��6#   	 �� Y1$ 0��6*   	 �� X2$ 45��61   	 �� Y2$ 45��69   ���	#FFF50000$��2��6T   $��Center=�6h   $
��Left=�6�   6�   �� 5E      -   5        $ 	MenuItem3��5E   %   #���   68   $
��Line��6e   ���	#FFF9F6F6$��Center=�6�   ���	#00DDDDDD$	��296��6�   $��44��6�   ���	#00DDDDDD$
 Line��6�   6�   5F      5G      5H      5I      o� 5J      $��24��6   $	��Top=�6   $��	0,0,886,0q�67   $��Right=�6J   $	��160��6f   .@�5K      m� -   5         m�Name$ StopResumeButton��5K      
���  ��$��40��6F   $��24��6Q   6~   m� 5L      -   5        $ Rewind��5L      
���   �$��40��6>   $��24��6I   6W   m� 5M      -   5        $ FlashForward��5M      
���   �$��40��6D   $��24��6O   6]   m� 5N      -   5        $ TimeManager��5N      
���   �$��40��6C   $��24��6N   6\   5O      5P      5Q      5R      