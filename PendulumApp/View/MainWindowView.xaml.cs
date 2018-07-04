﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SharpGL.SceneGraph;
using SharpGL;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PendulumApp.ViewModel;
using PendulumApp.ViewModel.OpenGLRender;
using PendulumApp.DeviceModel;
using PendulumApp.Settings;

namespace PendulumApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Device device;
        public MainWindow()
        {
            try
            {
                Log log = new Log();
                log.LogMessageToFile("Program started!!!");
                InitializeComponent();

                OpenGlDisplay openGLDisplay1 = new OpenGlDisplay(openGLControl1, 1f, 0f, 0f);
                OpenGlDisplay openGLDisplay2 = new OpenGlDisplay(openGLControl2, 1f, 0f, 0f);
                OpenGlDisplay openGLDisplay3 = new OpenGlDisplay(openGLControl3, 1f, 0f, 0f);
                OpenGlDisplay openGLDisplay4 = new OpenGlDisplay(openGLControl4, 1f, 0f, 0f);
                OpenGlDisplay openGLDisplay5 = new OpenGlDisplay(openGLControl5, 1f, 0f, 0f);
                OpenGlDisplay openGLDisplay6 = new OpenGlDisplay(openGLControl6, 1f, 0f, 0f);

                SettingProgram settingProgramDataHandler = SettingService.LoadSettingProgram();
                SettingEMG settingEMGDataHandler = SettingService.LoadSettingEMG();
                SettingACC settingACCDataHandler = SettingService.LoadSettingACC();
                SettingGY settingGYDataHandler = SettingService.LoadSettingGY();

                OpenGLDispatcher openGLDispatcherHandler = new OpenGLDispatcher(openGLDisplay1, openGLDisplay2, openGLDisplay3, openGLDisplay4, openGLDisplay5, openGLDisplay6, 2, 2, 3);
                MainWindowViewModel mainWindowHandler = new MainWindowViewModel(openGLDispatcherHandler, settingProgramDataHandler, settingEMGDataHandler, settingACCDataHandler, settingGYDataHandler);
                DataContext = mainWindowHandler;

                device = new Device(mainWindowHandler);
            }
            catch (Exception ex)
            {
                Log log = new Log();
                log.LogMessageToFile(ex.Message);
                Dialogs.DialogMessage.DialogMessageViewModel dvm = new Dialogs.DialogMessage.DialogMessageViewModel(Dialogs.DialogMessage.DialogImageTypeEnum.Error, ex.Message);
                Dialogs.DialogService.DialogResult result = Dialogs.DialogService.DialogService.OpenDialog(dvm);
            }
        }

        private void openGLControl1_Initialized(object sender, EventArgs e)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl1.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0.9f, 0.9f, 0.9f, 0);
        }
        private void openGLControl1_Resized(object sender, OpenGLEventArgs args)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl1.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            //gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);

            //gl.LookAt(-5, 5, -5, 0, 0, 0, 0, 1, 0);
            gl.Ortho(-0.2, 4.0, -0.2, 1.2, 1.0, -1.0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
        private void openGLControl2_Initialized(object sender, EventArgs e)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl2.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0.9f, 0.9f, 0.9f, 0);
        }
        private void openGLControl2_Resized(object sender, OpenGLEventArgs args)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl2.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            gl.Ortho(-0.2, 4.0, -0.2, 1.2, 1.0, -1.0);
            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
        private void openGLControl3_Initialized(object sender, EventArgs e)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl3.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0.9f, 0.9f, 0.9f, 0);
        }
        private void openGLControl3_Resized(object sender, OpenGLEventArgs args)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl3.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            //            gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);

            //  Use the 'look at' helper function to position and aim the camera.
            //            gl.LookAt(-5, 5, -5, 0, 0, 0, 0, 1, 0);
            gl.Ortho(-0.2, 4.0, -0.2, 1.2, 1.0, -1.0);
            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
        private void openGLControl4_Initialized(object sender, EventArgs e)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl4.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0.9f, 0.9f, 0.9f, 0);
        }
        private void openGLControl4_Resized(object sender, OpenGLEventArgs args)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl4.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            gl.Ortho(-0.2, 4.0, -0.2, 1.2, 1.0, -1.0);
            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
        private void openGLControl5_Initialized(object sender, EventArgs e)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl5.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0.9f, 0.9f, 0.9f, 0);
        }
        private void openGLControl5_Resized(object sender, OpenGLEventArgs args)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl5.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            gl.Ortho(-0.2, 4.0, -0.2, 1.2, 1.0, -1.0);
            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
        private void openGLControl6_Initialized(object sender, EventArgs e)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl6.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0.9f, 0.9f, 0.9f, 0);
        }
        private void openGLControl6_Resized(object sender, OpenGLEventArgs args)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl6.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            gl.Ortho(-0.2, 4.0, -0.2, 1.2, 1.0, -1.0);
            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
    }
}
