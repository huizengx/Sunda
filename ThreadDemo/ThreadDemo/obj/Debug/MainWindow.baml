   M S B A M L   `   `   `  ����E  AThreadDemo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:ThreadDemo
ThreadDemo  5   	       ThreadDemo.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:ThreadDemo     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      �  � ColumnDefinitions 5
   
   �� 5      �� 5       � RowDefinitions 5   
   �� 5      �� 5      .+�5   
   �� .+�5      �� -   5         ��Name$ ParallelBtn��5      .��6D   
Parallel6N   �� 5      -   5        $ TaskBtn��5      .��6<   Task6B   �� 5      -   5        $ Async_AwaitBtn��5      .��6J   Async_Await6W   �� 5      -   5        $ Thread_contend��5      .��6J   Thread_contend6Z   �� 5      -   5        $ DeadLock��5      .��6>   
DeadLock6H   5      �� 5   
   $��1��6   .+�5      �� -   5        $ ClearLogBtn��5      .��6D   Clear6K   5      5      5      