﻿using RPR.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RPR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected GameView game { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            game = new GameView(ref MainView);
        }

        private void MainView_Initialized(object sender, EventArgs e)
        {

        }

        async private void MainView_MouseEnter(object sender, MouseEventArgs e)
        {
            //MainView.Dispatcher.InvokeAsync(()=>
            //    game.Init());
        }

        private void MainView_MouseLeave(object sender, MouseEventArgs e)
        {
            //game.DeInit();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainView.Dispatcher.InvokeAsync(() =>
               game.Init());
        }
    }
}
