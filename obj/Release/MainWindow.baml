   M S B A M L   `   `   `  ���� A  =WpfApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WpfAppWpfApp  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem     WpfApp.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     &localclr-namespace:WpfApp     9System&clr-namespace:System;assembly=mscorlib     -     �Name$  FormTestApp�� � Title$% Тестовое задание��$��351.072��$	��718��.��� .+��� -    ��Name$ 	lblStatus��$��Статус: -��$
��Left=�$��
22,279,0,0q�$	��Top=�$��32��$	��178���� -    ��Name$ btnStart��$��Запустить��$
��Left=�$��246,282,0,0q�$	��Top=�$��75��$��19���� -    ��Name$ 	grBoxData��$E��?Настройки параметров подключения:��$	��258��$��24,7,0,0q�$	��Top=�$	��298�����	#FF474A4C
���   � J� OpacityMask
 �   �$
��Left=�.��� -    �Name$ gridSettings��$
��Left=�$	��241��$��-3,-2,-5.4,-3.8q�$	��Top=�$	��294��.+��� -   $ lblTypeRegister��$��Тип данных:��$
��Left=�$	��Top=�$��3,3,0,0q��� -    ��Name$ cbTypeRegister��$
��Left=�$	��Top=�$	��137��$��	104,8,0,0q�$��19�� �� SelectedIndex$ 0��$��10(�.@���Holding Registers����Input Registers����	Coils����Discrete Inputs���� -   $ lblIP��$��IP-адрес:��$
��Left=�$	��Top=�$��5,35,0,0q��� -	   	 ��Name$	 edtIP��$
��Left=�$��18��$��
103,39,0,0q�
 �� TextWrapping$

 Wrap=�$	��Top=�$	��137��$��10(��� -
   $ lblTCP��$��TCP-порт:��$
��Left=�$	��Top=�$��5,68,0,0q��� -   $	 edtTCP��$
��Left=�$��18��$��
104,72,0,0q�$

 Wrap=�$	��Top=�$	��137��$��10(��� -   $	 	edtClient��$
��Left=�$��18��$��104,105,0,0q�$

 Wrap=�$	��Top=�$	��137��$��10(��� -   $ lblCl��$��Клиенты(макс):��$
��Left=�$	��Top=�$��	5,100,0,0q��� -   $ btnUpdateData��$#��Обновить данные��$
��Left=�$��119,200,0,0q�$	��Top=�$	��108��{�.  �� -   $	 
edtRegAddr��$
��Left=�$��18��$��104,139,0,0q�$

 Wrap=�$	��Top=�$	��137��$��10(��� -   $ 
lblRegAddr��$��Рег.адрес:��$
��Left=�$	��Top=�$��	5,134,0,0q��� -   $	 edtRegValue��$
��Left=�$��18��$��104,173,0,0q�$

 Wrap=�$	��Top=�$	��137��$��10(��� -   $ lblRegValue��$��Рег.значение:��$
��Left=�$	��Top=�$��	5,168,0,0q�&   System.Windows.Controls.DataGrid  -     Name$ dtGrid��$
��Left=�$	��242��$	��Top=�$	��308��$��
358,20,0,0q�$ 0��   AutoGenerateColumns .     Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ Address��   
IsReadOnly .    MinWidth$	 100Q�1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *iAddr+   ClipboardContentBinding Z�   $ Value�� .    Width5  /System.Windows.Controls.DataGridLengthConverter$	 180 $	 200Q� �� *iValue+ Z� �� -   $	 edtStartValue��$
��Left=�$��18��$��422,283,0,0q�$

 Wrap=�$	��Top=�$��60��$��10(�$��1�� �� 	UndoLimit$ 65535���� -   $ lblStartValue��$��Начать с:��$
��Left=�$	��Top=�$��360,278,0,0q�$��26��$��59���� -   $	 edtStep��$
��Left=�$��18��$��561,283,0,0q�$

 Wrap=�$	��Top=�$��60��$��10(�$��10���� -   $ lblStep��$��Шаг:��$
��Left=�$	��Top=�$��516,279,0,0q�$��26��$��35��