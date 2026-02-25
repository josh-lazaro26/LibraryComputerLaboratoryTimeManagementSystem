using System.Drawing;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.FORMS
{
    public static class MainFormResponsiveLayout
    {
        private static float _scaleX;
        private static float _scaleY;

        private const float BASE_WIDTH = 1280f;
        private const float BASE_HEIGHT = 780f;

        public static void Initialize(int screenWidth, int screenHeight)
        {
            _scaleX = screenWidth / BASE_WIDTH;
            _scaleY = screenHeight / BASE_HEIGHT;
        }

        public static Font ScaleFont(string fontFamily, float baseFontSize, FontStyle style = FontStyle.Regular)
        {
            return new Font(fontFamily, baseFontSize * _scaleY, style);
        }

        public static Size ScaleSize(int baseWidth, int baseHeight)
        {
            return new Size((int)(baseWidth * _scaleX), (int)(baseHeight * _scaleY));
        }

        public static Point ScaleLocation(int baseX, int baseY)
        {
            return new Point((int)(baseX * _scaleX), (int)(baseY * _scaleY));
        }

        public static int CenterHorizontal(int screenWidth, int controlWidth)
        {
            return (screenWidth - controlWidth) / 2;
        }

        public static void Apply(MainForm form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;

            Initialize(w, h);

            int scaledExpandedWidth = (int)(300 * _scaleX);
            int scaledCollapsedWidth = (int)(55 * _scaleX);

            form._sidebarAnimator.UpdateWidths(scaledExpandedWidth, scaledCollapsedWidth);
            form.SidebarPanel.Width = form._sidebarAnimator.IsExpanded
                ? scaledExpandedWidth
                : scaledCollapsedWidth;

            ApplyHeader(form, w);
            ApplySidebar(form, h, scaledExpandedWidth, scaledCollapsedWidth);
            ApplyDashboardPanel(form, w, h);
            ApplyStudentRegistrationPanel(form);
            ApplyListOfStudentsPanel(form);
            ApplyAdminCreationPanel(form);
            ApplyTimeManagementPanel(form);
            UpdatePanelAnimations(form);
            ApplyAdminReportsPanel(form); 
        }

        // ── Header ────────────────────────────────────────────────────────────
        private static void ApplyHeader(MainForm form, int w)
        {
            form.HeaderPanel.Size = new Size(w, (int)(71 * _scaleY));

            form.HeaderPanelLabel.Font = ScaleFont("Roboto Condensed", 20f);
            form.HeaderPanelLabel.Size = ScaleSize(640, 33);
            form.HeaderPanelLabel.Location = ScaleLocation(371, 22);

            form.SidebarBtn.Size = ScaleSize(48, 43);
            form.SidebarBtn.Location = ScaleLocation(4, 12);

            form.MinimizeBtn.Size = ScaleSize(44, 46);
            form.MinimizeBtn.Location = new Point(w - (int)(143 * _scaleX), (int)(13 * _scaleY));

            form.MaximizeBtn.Size = ScaleSize(44, 46);
            form.MaximizeBtn.Location = new Point(w - (int)(96 * _scaleX), (int)(12 * _scaleY));

            form.CloseBtn.Size = ScaleSize(44, 46);
            form.CloseBtn.Location = new Point(w - (int)(50 * _scaleX), (int)(12 * _scaleY));
        }

        // ── Sidebar ───────────────────────────────────────────────────────────
        private static void ApplySidebar(MainForm form, int h, int expandedWidth, int collapsedWidth)
        {
            bool isExpanded = form._sidebarAnimator.IsExpanded;

            form.SidebarPanel.Size = new Size(
                isExpanded ? expandedWidth : collapsedWidth,
                h - form.HeaderPanel.Height
            );

            if (isExpanded)
            {
                form.pictureBox1.Visible = true;
                form.pictureBox1.Size = ScaleSize(235, 160);
                form.pictureBox1.Location = ScaleLocation(33, 19);

                ScaleSidebarBtn(form.DashboardSidebarBtn, 0, 201);
                ScaleSidebarBtn(form.StudentManagementSidebarBtn, 0, 261);
                ScaleSidebarBtn(form.ListOfStudentsSidebarBtn, 0, 321);
                ScaleSidebarBtn(form.TimeManagementSidebarBtn, 0, 381);
                ScaleSidebarBtn(form.AdminCreation, 0, 441);
                ScaleSidebarBtn(form.ReportBtn, 0, 501);
                ScaleSidebarBtn(form.LogoutBtn, 0, 648);
            }
            else
            {
                form.pictureBox1.Visible = true;
                form.pictureBox1.Size = ScaleSize(235, 160);
                form.pictureBox1.Location = ScaleLocation(33, 19);
                ScaleCollapsedSidebarBtn(form.DashboardSidebarBtn, 0, 201);
                ScaleCollapsedSidebarBtn(form.StudentManagementSidebarBtn, 0, 261);
                ScaleCollapsedSidebarBtn(form.ListOfStudentsSidebarBtn, 0, 321);
                ScaleCollapsedSidebarBtn(form.TimeManagementSidebarBtn, 0, 381);
                ScaleCollapsedSidebarBtn(form.AdminCreation, 0, 441);
                ScaleCollapsedSidebarBtn(form.ReportBtn, 0, 501);
                ScaleCollapsedSidebarBtn(form.LogoutBtn, 0, 648);
            }
        }

        private static void ScaleSidebarBtn(Button btn, int baseX, int baseY)
        {
            btn.Size = ScaleSize(300, 57);
            btn.Font = ScaleFont("Inter SemiBold", 18f, FontStyle.Bold);
            btn.Location = ScaleLocation(baseX, baseY);
            btn.Padding = new Padding(15, 0, 0, 0);
        }

        private static void ScaleCollapsedSidebarBtn(Button btn, int baseX, int baseY)
        {
            btn.Size = ScaleSize(55, 57);
            btn.Font = ScaleFont("Inter SemiBold", 18f, FontStyle.Bold);
            btn.Location = ScaleLocation(baseX, baseY);
            btn.Padding = new Padding(15, 0, 0, 0);
        }

        // ── Dashboard Panel ───────────────────────────────────────────────────
        private static void ApplyDashboardPanel(MainForm form, int w, int h)
        {
            form.WelcomeAdminPanel.Size = ScaleSize(823, 220);
            form.WelcomeAdminPanel.Location = ScaleLocation(385, 229);

            form.WelcomeAdminLabel.Font = ScaleFont("Roboto", 36f, FontStyle.Bold);
            form.WelcomeAdminLabel.Size = ScaleSize(520, 58);
            form.WelcomeAdminLabel.Location = ScaleLocation(160, 34);

            form.label6.Font = ScaleFont("Roboto Black", 15f, FontStyle.Bold);
            form.label6.Location = new Point(w - (int)(210 * _scaleX), (int)(95 * _scaleY));
        }

        // ── Student Registration Panel ────────────────────────────────────────
        private static void ApplyStudentRegistrationPanel(MainForm form)
        {
            form.RegisterStudentFieldPanel.Size = ScaleSize(966, 590);
            form.RegisterStudentFieldPanel.Location = ScaleLocation(299, 112);

            ScaleLabel(form.StudentNameLabel, "Roboto Condensed", 18f, FontStyle.Bold, 95, 70);
            ScaleLabel(form.StudentIDLabel, "Roboto Condensed", 18f, FontStyle.Bold, 528, 70);
            ScaleLabel(form.label12, "Roboto Condensed", 18f, FontStyle.Bold, 95, 189);
            ScaleLabel(form.CourseLabel, "Roboto Condensed", 18f, FontStyle.Bold, 528, 191);
            ScaleLabel(form.label13, "Roboto Condensed", 18f, FontStyle.Bold, 95, 314);
            ScaleLabel(form.label10, "Roboto Condensed", 18f, FontStyle.Bold, 528, 278);

            ScaleInputPanel(form.FullNameTbPanelR, form.StudentFirstNameTb, 97, 104, 362, 60);
            ScaleInputPanel(form.StudentIDTbPanelR, form.StudentIDTb, 533, 104, 362, 60);
            ScaleInputPanel(form.MiddleNameTbPanelR, form.StudentMiddleNameTb, 97, 226, 362, 60);
            ScaleInputPanel(form.LastNameTbPanelR, form.StudentLastNameTb, 97, 346, 362, 60);

            form.StudentCourseCb.Font = ScaleFont("Roboto", 18f);
            form.StudentCourseCb.Size = ScaleSize(241, 37);
            form.StudentCourseCb.Location = ScaleLocation(654, 188);

            form.StudentYearLevelCb.Font = ScaleFont("Roboto", 18f);
            form.StudentYearLevelCb.Size = ScaleSize(241, 37);
            form.StudentYearLevelCb.Location = ScaleLocation(655, 273);

            ScaleActionBtn(form.RegisterRfidBtn, 536, 346, 360, 56);
            ScaleActionBtn(form.RegisterStudentBtn, 306, 477, 362, 56);
        }

        // ── List of Students Panel ────────────────────────────────────────────
        private static void ApplyListOfStudentsPanel(MainForm form)
        {
            form.StudentsTablePanel.Size = ScaleSize(513, 608);
            form.StudentsTablePanel.Location = ScaleLocation(732, 127);

            form.ListOfStudentDgv.Size = ScaleSize(475, 489);
            form.ListOfStudentDgv.Location = ScaleLocation(17, 16);
            form.ListOfStudentDgv.Font = ScaleFont("Roboto", 10f);

            ScaleActionBtn(form.UpdateStudentBtn, 17, 531, 199, 56);
            ScaleActionBtn(form.DeleteStudentBtn, 293, 531, 199, 56);

            form.StudentFieldsPanel.Size = ScaleSize(355, 558);
            form.StudentFieldsPanel.Location = ScaleLocation(355, 127);

            ScaleLabel(form.label14, "Roboto Condensed", 18f, FontStyle.Bold, 21, 5);
            ScaleLabel(form.label15, "Roboto Condensed", 18f, FontStyle.Bold, 21, 108);
            ScaleLabel(form.label16, "Roboto Condensed", 18f, FontStyle.Bold, 21, 209);
            ScaleLabel(form.label17, "Roboto Condensed", 18f, FontStyle.Bold, 21, 319);
            ScaleLabel(form.label18, "Roboto Condensed", 18f, FontStyle.Bold, 20, 517);
            ScaleLabel(form.label19, "Roboto Condensed", 18f, FontStyle.Bold, 21, 439);

            ScaleInputPanel(form.FirstNameTbPanelL, form.ListOfStudentFirstNameTb, 26, 37, 306, 60);
            ScaleInputPanel(form.MiddleNameTbPanelL, form.ListOfStudentMiddleNameTb, 26, 140, 306, 60);
            ScaleInputPanel(form.LastNameTbPanelL, form.ListOfStudentLastNameTb, 25, 244, 306, 60);
            ScaleInputPanel(form.StudentIdTbPanelL, form.ListOfStudentStudentIdTb, 26, 351, 306, 60);

            form.ListOfStudentCourseCb.Font = ScaleFont("Roboto", 18f);
            form.ListOfStudentCourseCb.Size = ScaleSize(228, 37);
            form.ListOfStudentCourseCb.Location = ScaleLocation(103, 436);

            form.ListOfStudentYearLevelCb.Font = ScaleFont("Roboto", 18f);
            form.ListOfStudentYearLevelCb.Size = ScaleSize(194, 37);
            form.ListOfStudentYearLevelCb.Location = ScaleLocation(137, 512);
        }

        // ── Admin Creation Panel ──────────────────────────────────────────────
        private static void ApplyAdminCreationPanel(MainForm form)
        {
            form.AdminFields.Size = ScaleSize(400, 600);
            form.AdminFields.Location = ScaleLocation(358, 124);

            ScaleLabel(form.label9, "Roboto Condensed", 18f, FontStyle.Bold, 14, 12);
            ScaleLabel(form.label8, "Roboto Condensed", 18f, FontStyle.Bold, 14, 111);
            ScaleLabel(form.label5, "Roboto Condensed", 18f, FontStyle.Bold, 14, 210);
            ScaleLabel(form.label4, "Roboto Condensed", 18f, FontStyle.Bold, 14, 309);
            ScaleLabel(form.label2, "Roboto Condensed", 18f, FontStyle.Bold, 14, 408);

            ScaleInputPanel(form.PersonnelIdTbPanelAc, form.IDPersonnelTb, 19, 46, 362, 60);
            ScaleInputPanel(form.FirstNameTbPanelAc, form.AdminFirstNameTb, 19, 145, 362, 60);
            ScaleInputPanel(form.MiddleNameTbPanelAc, form.AdminMiddleNameTb, 19, 244, 362, 60);
            ScaleInputPanel(form.LastNameTbPanelAc, form.AdminLastNameTb, 19, 343, 362, 60);
            ScaleInputPanel(form.RfidTbPanelAc, form.AdminRfidTb, 19, 442, 362, 60);

            ScaleActionBtn(form.RegisterAdminBtn, 19, 528, 362, 56);

            form.AdminTablePanel.Size = ScaleSize(460, 600);
            form.AdminTablePanel.Location = ScaleLocation(784, 124);

            form.AdminDgv.Size = ScaleSize(436, 499);
            form.AdminDgv.Location = ScaleLocation(13, 14);
            form.AdminDgv.Font = ScaleFont("Roboto", 10f);

            ScaleActionBtn(form.UpdateAdmin_Btn, 13, 528, 199, 56);
            ScaleActionBtn(form.DeleteAdmin_Btn, 250, 528, 199, 56);
        }

        // ── Time Management Panel ─────────────────────────────────────────────
        private static void ApplyTimeManagementPanel(MainForm form)
        {
            form.label3.Font = ScaleFont("Roboto Medium", 27f, FontStyle.Bold);
            form.label3.Location = ScaleLocation(627, 25);

            ScaleTimeBtnPanel(
                form.TimeManagementBtnPanel1, ScaleLocation(308, 123),
                new[] { form.StudentTimeBtn1, form.StudentTimeBtn5, form.StudentTimeBtn9, form.StudentTimeBtn13 },
                new[] { form.StudentBtnLabel1, form.StudentBtnLabel5, form.StudentBtnLabel9, form.StudentBtnLabel13 }
            );

            ScaleTimeBtnPanel(
                form.TimeManagementBtnPanel2, ScaleLocation(555, 123),
                new[] { form.StudentTimeBtn2, form.StudentTimeBtn6, form.StudentTimeBtn10, form.StudentTimeBtn14 },
                new[] { form.StudentBtnLabel2, form.StudentBtnLabel6, form.StudentBtnLabel10, form.StudentBtnLabel14 }
            );

            ScaleTimeBtnPanel(
                form.TimeManagementBtnPanel3, ScaleLocation(802, 123),
                new[] { form.StudentTimeBtn3, form.StudentTimeBtn7, form.StudentTimeBtn11, form.StudentTimeBtn15 },
                new[] { form.StudentBtnLabel3, form.StudentBtnLabel7, form.StudentBtnLabel11, form.StudentBtnLabel15 }
            );

            ScaleTimeBtnPanel(
                form.TimeManagementBtnPanel4, ScaleLocation(1050, 123),
                new[] { form.StudentTimeBtn4, form.StudentTimeBtn8, form.StudentTimeBtn12, form.StudentTimeBtn16 },
                new[] { form.StudentBtnLabel4, form.StudentBtnLabel8, form.StudentBtnLabel12, form.StudentBtnLabel16 }
            );
        }

        // ── Shared Helpers ────────────────────────────────────────────────────
        private static void ScaleLabel(Label lbl, string font, float size, FontStyle style, int baseX, int baseY)
        {
            lbl.Font = ScaleFont(font, size, style);
            lbl.Location = ScaleLocation(baseX, baseY);
        }

        private static void ScaleInputPanel(Panel panel, TextBox textBox, int baseX, int baseY, int baseW, int baseH)
        {
            panel.Size = ScaleSize(baseW, baseH);
            panel.Location = ScaleLocation(baseX, baseY);
            textBox.Font = ScaleFont("Roboto", 18f);
            textBox.Size = new Size(panel.Width - 20, (int)(29 * _scaleY));
            textBox.Location = new Point(10, (panel.Height - textBox.Height) / 2);
        }

        private static void ScaleActionBtn(Button btn, int baseX, int baseY, int baseW, int baseH)
        {
            btn.Size = ScaleSize(baseW, baseH);
            btn.Location = ScaleLocation(baseX, baseY);
            btn.Font = ScaleFont("Roboto Black", 21f, FontStyle.Bold);
        }

        private static void ScaleTimeBtnPanel(Panel panel, Point location, Button[] buttons, Label[] labels)
        {
            panel.Location = location;
            panel.Size = ScaleSize(235, 586);

            int[] buttonY = { 6, 155, 305, 458 };
            int[] labelY = { 71, 223, 370, 522 };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Size = ScaleSize(200, 120);
                buttons[i].Location = ScaleLocation(15, buttonY[i]);
                buttons[i].Font = ScaleFont("Roboto Condensed", 12f);

                labels[i].Font = ScaleFont("Roboto Condensed", 20f);
                labels[i].Location = ScaleLocation(65, labelY[i]); // was 86
            }
        }

        public static void UpdatePanelAnimations(MainForm form)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            Initialize(w, h);

            if (form.AdminCreationPanel.Visible)
            {
                form._panelAnimator?.Clear();
                form._panelAnimator?.AddPanel(
                    form.AdminFields,
                    ScaleLocation(358, 124),
                    ScaleLocation(224, 124)
                );
                form._panelAnimator?.AddPanel(
                    form.AdminTablePanel,
                    ScaleLocation(784, 124),
                    ScaleLocation(673, 124)
                );
            }
            else if (form.TimeManagementPanel.Visible)
            {
                form._panelAnimator?.Clear();
                form._panelAnimator?.AddPanel(form.TimeManagementBtnPanel1, ScaleLocation(308, 123), ScaleLocation(162, 123));
                form._panelAnimator?.AddPanel(form.TimeManagementBtnPanel2, ScaleLocation(555, 123), ScaleLocation(442, 123));
                form._panelAnimator?.AddPanel(form.TimeManagementBtnPanel3, ScaleLocation(802, 123), ScaleLocation(716, 123));
                form._panelAnimator?.AddPanel(form.TimeManagementBtnPanel4, ScaleLocation(1050, 123), ScaleLocation(994, 123));
            }
            else if (form.ListOfStudentsPanel.Visible)
            {
                form._panelAnimator?.Clear();
                form._panelAnimator?.AddPanel(form.StudentFieldsPanel, ScaleLocation(355, 127), ScaleLocation(258, 127));
                form._panelAnimator?.AddPanel(form.StudentsTablePanel, ScaleLocation(732, 127), ScaleLocation(644, 127));
            }
            else if (form.StudentPanel.Visible)
            {
                form._panelAnimator?.Clear();
                form._panelAnimator?.AddPanel(form.RegisterStudentFieldPanel, ScaleLocation(299, 112), ScaleLocation(167, 112));
            }
            else if (form.DashboardPanel.Visible)
            {
                form._panelAnimator?.Clear();
                form._panelAnimator?.AddPanel(form.WelcomeAdminPanel, ScaleLocation(385, 229), ScaleLocation(261, 229));
            }
            else if (form.AdminReportsPanel.Visible)
            {
                form._panelAnimator?.Clear();
                form._panelAnimator?.AddPanel(
                    form.ReportDgvPanel,
                    ScaleLocation(311, 83),
                    ScaleLocation(193, 83)
                );
            }
        }

        public static void RestoreNormalLayout(MainForm form)
        {
            // ── Reset SidebarAnimator ─────────────────────────────────────
            form._sidebarAnimator.UpdateWidths(300, 55, 8);

            bool isExpanded = form._sidebarAnimator.IsExpanded;

            form.SidebarPanel.Width = isExpanded ? 300 : 55;
            form.SidebarPanel.Size = new Size(
                isExpanded ? 300 : 55,
                form.ClientSize.Height - 71
            );

            // ── Sidebar Controls ──────────────────────────────────────────
            if (isExpanded)
            {
                form.pictureBox1.Visible = true;
                form.pictureBox1.Size = new Size(235, 160);
                form.pictureBox1.Location = new Point(33, 19);
                RestoreNormalSidebarBtn(form.DashboardSidebarBtn, 0, 201, 8);
                RestoreNormalSidebarBtn(form.StudentManagementSidebarBtn, 0, 261, 8);
                RestoreNormalSidebarBtn(form.ListOfStudentsSidebarBtn, 0, 321, 8);
                RestoreNormalSidebarBtn(form.TimeManagementSidebarBtn, 0, 381, 8);
                RestoreNormalSidebarBtn(form.AdminCreation, 0, 441, 8);
                RestoreNormalSidebarBtn(form.ReportBtn, 0, 501, 8);
                RestoreNormalSidebarBtn(form.LogoutBtn, 0, 648, 8);
            }
            else
            {
                form.pictureBox1.Visible = true;
                form.pictureBox1.Size = new Size(235, 160);
                form.pictureBox1.Location = new Point(33, 19);
                RestoreCollapsedSidebarBtn(form.DashboardSidebarBtn, 0, 201, 5);
                RestoreCollapsedSidebarBtn(form.StudentManagementSidebarBtn, 0, 261,5);
                RestoreCollapsedSidebarBtn(form.ListOfStudentsSidebarBtn, 0, 321, 5);
                RestoreCollapsedSidebarBtn(form.TimeManagementSidebarBtn, 0, 381, 5);
                RestoreCollapsedSidebarBtn(form.AdminCreation, 0, 441, 5);
                RestoreCollapsedSidebarBtn(form.ReportBtn, 0, 501, 5);
                RestoreCollapsedSidebarBtn(form.LogoutBtn, 0, 648, 5);
            }

            // ── Header ────────────────────────────────────────────────────
            form.HeaderPanel.Size = new Size(1280, 71);
            form.HeaderPanelLabel.Font = new Font("Roboto Condensed", 20.25f);
            form.HeaderPanelLabel.Size = new Size(640, 33);
            form.HeaderPanelLabel.Location = new Point(371, 22);
            form.SidebarBtn.Size = new Size(48, 43);
            form.SidebarBtn.Location = new Point(4, 12);
            form.MinimizeBtn.Size = new Size(44, 46);
            form.MinimizeBtn.Location = new Point(1137, 13);
            form.MaximizeBtn.Size = new Size(44, 46);
            form.MaximizeBtn.Location = new Point(1184, 12);
            form.CloseBtn.Size = new Size(44, 46);
            form.CloseBtn.Location = new Point(1230, 12);

            // ── Dashboard ─────────────────────────────────────────────────
            form.WelcomeAdminPanel.Size = new Size(823, 220);
            form.WelcomeAdminPanel.Location = new Point(385, 229);
            form.WelcomeAdminLabel.Font = new Font("Roboto", 36f, FontStyle.Bold);
            form.WelcomeAdminLabel.Size = new Size(520, 58);
            form.WelcomeAdminLabel.Location = new Point(160, 34);
            form.label6.Font = new Font("Roboto Black", 15.75f, FontStyle.Bold);
            form.label6.Location = new Point(1070, 95);

            // ── Student Registration ──────────────────────────────────────
            form.RegisterStudentFieldPanel.Size = new Size(966, 590);
            form.RegisterStudentFieldPanel.Location = new Point(299, 112);
            RestoreLabel(form.StudentNameLabel, "Roboto Condensed", 18f, FontStyle.Bold, 95, 70);
            RestoreLabel(form.StudentIDLabel, "Roboto Condensed", 18f, FontStyle.Bold, 528, 70);
            RestoreLabel(form.label12, "Roboto Condensed", 18f, FontStyle.Bold, 95, 189);
            RestoreLabel(form.CourseLabel, "Roboto Condensed", 18f, FontStyle.Bold, 528, 191);
            RestoreLabel(form.label13, "Roboto Condensed", 18f, FontStyle.Bold, 95, 314);
            RestoreLabel(form.label10, "Roboto Condensed", 18f, FontStyle.Bold, 528, 278);
            RestoreInputPanel(form.FullNameTbPanelR, form.StudentFirstNameTb, 97, 104, 362, 60);
            RestoreInputPanel(form.StudentIDTbPanelR, form.StudentIDTb, 533, 104, 362, 60);
            RestoreInputPanel(form.MiddleNameTbPanelR, form.StudentMiddleNameTb, 97, 226, 362, 60);
            RestoreInputPanel(form.LastNameTbPanelR, form.StudentLastNameTb, 97, 346, 362, 60);
            form.StudentCourseCb.Font = new Font("Roboto", 18f);
            form.StudentCourseCb.Size = new Size(241, 37);
            form.StudentCourseCb.Location = new Point(654, 188);
            form.StudentYearLevelCb.Font = new Font("Roboto", 18f);
            form.StudentYearLevelCb.Size = new Size(241, 37);
            form.StudentYearLevelCb.Location = new Point(655, 273);
            RestoreActionBtn(form.RegisterRfidBtn, 536, 346, 360, 56);
            RestoreActionBtn(form.RegisterStudentBtn, 306, 477, 362, 56);
            // ── List of Students ──────────────────────────────────────────
            form.StudentFieldsPanel.Size = new Size(355, 558);
            form.StudentFieldsPanel.Location = new Point(355, 127);
            form.StudentsTablePanel.Size = new Size(513, 608);
            form.StudentsTablePanel.Location = new Point(732, 127);
            form.ListOfStudentDgv.Size = new Size(475, 489);
            form.ListOfStudentDgv.Location = new Point(17, 16);
            form.ListOfStudentDgv.Font = new Font("Roboto", 10f);
            RestoreActionBtn(form.UpdateStudentBtn, 17, 531, 199, 56);
            RestoreActionBtn(form.DeleteStudentBtn, 293, 531, 199, 56);
            RestoreLabel(form.label14, "Roboto Condensed", 18f, FontStyle.Bold, 21, 5);
            RestoreLabel(form.label15, "Roboto Condensed", 18f, FontStyle.Bold, 21, 108);
            RestoreLabel(form.label16, "Roboto Condensed", 18f, FontStyle.Bold, 21, 209);
            RestoreLabel(form.label17, "Roboto Condensed", 18f, FontStyle.Bold, 21, 319);
            RestoreLabel(form.label18, "Roboto Condensed", 18f, FontStyle.Bold, 20, 517);
            RestoreLabel(form.label19, "Roboto Condensed", 18f, FontStyle.Bold, 21, 439);
            RestoreInputPanel(form.FirstNameTbPanelL, form.ListOfStudentFirstNameTb, 26, 37, 306, 60);
            RestoreInputPanel(form.MiddleNameTbPanelL, form.ListOfStudentMiddleNameTb, 26, 140, 306, 60);
            RestoreInputPanel(form.LastNameTbPanelL, form.ListOfStudentLastNameTb, 25, 244, 306, 60);
            RestoreInputPanel(form.StudentIdTbPanelL, form.ListOfStudentStudentIdTb, 26, 351, 306, 60);
            form.ListOfStudentCourseCb.Font = new Font("Roboto", 18f);
            form.ListOfStudentCourseCb.Size = new Size(228, 37);
            form.ListOfStudentCourseCb.Location = new Point(103, 436);
            form.ListOfStudentYearLevelCb.Font = new Font("Roboto", 18f);
            form.ListOfStudentYearLevelCb.Size = new Size(194, 37);
            form.ListOfStudentYearLevelCb.Location = new Point(137, 512);

            // ── Admin Creation ────────────────────────────────────────────
            form.AdminFields.Size = new Size(400, 600);
            form.AdminFields.Location = new Point(358, 124);
            RestoreLabel(form.label9, "Roboto Condensed", 18f, FontStyle.Bold, 14, 12);
            RestoreLabel(form.label8, "Roboto Condensed", 18f, FontStyle.Bold, 14, 111);
            RestoreLabel(form.label5, "Roboto Condensed", 18f, FontStyle.Bold, 14, 210);
            RestoreLabel(form.label4, "Roboto Condensed", 18f, FontStyle.Bold, 14, 309);
            RestoreLabel(form.label2, "Roboto Condensed", 18f, FontStyle.Bold, 14, 408);
            RestoreInputPanel(form.PersonnelIdTbPanelAc, form.IDPersonnelTb, 19, 46, 362, 60);
            RestoreInputPanel(form.FirstNameTbPanelAc, form.AdminFirstNameTb, 19, 145, 362, 60);
            RestoreInputPanel(form.MiddleNameTbPanelAc, form.AdminMiddleNameTb, 19, 244, 362, 60);
            RestoreInputPanel(form.LastNameTbPanelAc, form.AdminLastNameTb, 19, 343, 362, 60);
            RestoreInputPanel(form.RfidTbPanelAc, form.AdminRfidTb, 19, 442, 362, 60);
            RestoreActionBtn(form.RegisterAdminBtn, 19, 528, 362, 56);
            form.AdminTablePanel.Size = new Size(460, 600);
            form.AdminTablePanel.Location = new Point(784, 124);
            form.AdminDgv.Size = new Size(436, 499);
            form.AdminDgv.Location = new Point(13, 14);
            form.AdminDgv.Font = new Font("Roboto", 10f);
            RestoreActionBtn(form.UpdateAdmin_Btn, 13, 528, 199, 56);
            RestoreActionBtn(form.DeleteAdmin_Btn, 250, 528, 199, 56);

            // ── Time Management ───────────────────────────────────────────
            form.label3.Font = new Font("Roboto Medium", 27.75f, FontStyle.Bold);
            form.label3.Location = new Point(627, 25);
            RestoreTimeBtnPanel(
                form.TimeManagementBtnPanel1, new Point(308, 123),
                new[] { form.StudentTimeBtn1, form.StudentTimeBtn5, form.StudentTimeBtn9, form.StudentTimeBtn13 },
                new[] { form.StudentBtnLabel1, form.StudentBtnLabel5, form.StudentBtnLabel9, form.StudentBtnLabel13 }
            );
            RestoreTimeBtnPanel(
                form.TimeManagementBtnPanel2, new Point(555, 123),
                new[] { form.StudentTimeBtn2, form.StudentTimeBtn6, form.StudentTimeBtn10, form.StudentTimeBtn14 },
                new[] { form.StudentBtnLabel2, form.StudentBtnLabel6, form.StudentBtnLabel10, form.StudentBtnLabel14 }
            );
            RestoreTimeBtnPanel(
                form.TimeManagementBtnPanel3, new Point(802, 123),
                new[] { form.StudentTimeBtn3, form.StudentTimeBtn7, form.StudentTimeBtn11, form.StudentTimeBtn15 },
                new[] { form.StudentBtnLabel3, form.StudentBtnLabel7, form.StudentBtnLabel11, form.StudentBtnLabel15 }
            );
            RestoreTimeBtnPanel(
                form.TimeManagementBtnPanel4, new Point(1050, 123),
                new[] { form.StudentTimeBtn4, form.StudentTimeBtn8, form.StudentTimeBtn12, form.StudentTimeBtn16 },
                new[] { form.StudentBtnLabel4, form.StudentBtnLabel8, form.StudentBtnLabel12, form.StudentBtnLabel16 }
            );

            // ── Admin Reports ─────────────────────────────────────────────
            form.ReportDgvPanel.Size = new Size(955, 675);
            form.ReportDgvPanel.Location = new Point(311, 83);
            form.ReportsDataGridView.Dock = DockStyle.None;
            form.ReportsDataGridView.Size = new Size(951, 500); // reduced from 595
            form.ReportsDataGridView.Location = new Point(2, 50); // pushed down 40px from top
            form.ReportsDataGridView.Font = new Font("Roboto", 10f);
            form.PrintReportBtn.Size = new Size(268, 56);
            form.PrintReportBtn.Location = new Point((955 - 268) / 2, 675 - 56 - 10); // (343, 609)
            form.PrintReportBtn.Font = new Font("Roboto Black", 21.75f, FontStyle.Bold);
            // ── Register animations and snap ──────────────────────────────
            form._panelAnimator?.Clear();

            if (form.AdminCreationPanel.Visible)
            {
                form._panelAnimator?.AddPanel(form.AdminFields, new Point(358, 124), new Point(224, 124));
                form._panelAnimator?.AddPanel(form.AdminTablePanel, new Point(784, 124), new Point(673, 124));
            }
            else if (form.TimeManagementPanel.Visible)
            {
                form._panelAnimator?.AddPanel(form.TimeManagementBtnPanel1, new Point(308, 123), new Point(162, 123));
                form._panelAnimator?.AddPanel(form.TimeManagementBtnPanel2, new Point(555, 123), new Point(442, 123));
                form._panelAnimator?.AddPanel(form.TimeManagementBtnPanel3, new Point(802, 123), new Point(716, 123));
                form._panelAnimator?.AddPanel(form.TimeManagementBtnPanel4, new Point(1050, 123), new Point(994, 123));
            }
            else if (form.ListOfStudentsPanel.Visible)
            {
                form._panelAnimator?.AddPanel(form.StudentFieldsPanel, new Point(355, 127), new Point(258, 127));
                form._panelAnimator?.AddPanel(form.StudentsTablePanel, new Point(732, 127), new Point(644, 127));
            }
            else if (form.StudentPanel.Visible)
            {
                form._panelAnimator?.AddPanel(form.RegisterStudentFieldPanel, new Point(299, 112), new Point(167, 112));
            }
            else if (form.DashboardPanel.Visible)
            {
                form._panelAnimator?.AddPanel(form.WelcomeAdminPanel, new Point(385, 229), new Point(261, 229));
            }
            else if (form.AdminReportsPanel.Visible)
            {
                form._panelAnimator?.AddPanel(
                    form.ReportDgvPanel,
                    new Point(311, 83),
                    new Point(193, 83)
                );
            }

            form._panelAnimator?.SnapToState(!isExpanded);
        }

        // ── Restore Helpers ───────────────────────────────────────────────────
        private static void RestoreNormalSidebarBtn(Button btn, int x, int y, int padding)
        {
            btn.Size = new Size(300, 57);
            btn.Font = new Font("Inter SemiBold", 18f, FontStyle.Bold);
            btn.Location = new Point(x, y);
            btn.Padding = new Padding(padding, 0, 0, 0);
        }

        private static void RestoreCollapsedSidebarBtn(Button btn, int x, int y, int padding)
        {
            btn.Size = new Size(55, 57);
            btn.Font = new Font("Inter SemiBold", 18f, FontStyle.Bold);
            btn.Location = new Point(x, y);
            btn.Padding = new Padding(padding, 0, 0, 0);
        }

        private static void RestoreLabel(Label lbl, string fontFamily, float size, FontStyle style, int x, int y)
        {
            lbl.Font = new Font(fontFamily, size, style);
            lbl.Location = new Point(x, y);
        }

        private static void RestoreInputPanel(Panel panel, TextBox textBox, int x, int y, int w, int h)
        {
            panel.Size = new Size(w, h);
            panel.Location = new Point(x, y);
            textBox.Font = new Font("Roboto", 18f);
            textBox.Size = new Size(w - 20, 29);
            textBox.Location = new Point(10, (h - 29) / 2);
        }

        private static void RestoreActionBtn(Button btn, int x, int y, int w, int h)
        {
            btn.Size = new Size(w, h);
            btn.Location = new Point(x, y);
            btn.Font = new Font("Roboto Black", 21.75f, FontStyle.Bold);
        }
        private static void ApplyAdminReportsPanel(MainForm form)
        {
            form.ReportDgvPanel.Size = ScaleSize(955, 675);
            form.ReportDgvPanel.Location = ScaleLocation(311, 83);

            int btnH = (int)(56 * _scaleY);
            int padding = (int)(10 * _scaleY);

            form.ReportsDataGridView.Size = new Size(
                form.ReportDgvPanel.Width - 4,
                form.ReportDgvPanel.Height - btnH - (padding * 3) - (int)(50 * _scaleY) // extra space at top
            );
            form.ReportsDataGridView.Location = new Point(2, (int)(50 * _scaleY)); // push down
            form.ReportsDataGridView.Font = ScaleFont("Roboto", 10f);

            int btnW = (int)(form.ReportDgvPanel.Width * 0.22);

            int btnX = (form.ReportDgvPanel.Width - btnW) / 2;
            int btnY = form.ReportDgvPanel.Height - btnH - padding;
            form.PrintReportBtn.Size = new Size(btnW, btnH);
            form.PrintReportBtn.Location = new Point(btnX, btnY);
            form.PrintReportBtn.Font = ScaleFont("Roboto Black", 17f, FontStyle.Bold);
        }
        private static void RestoreTimeBtnPanel(Panel panel, Point location, Button[] buttons, Label[] labels)
        {
            panel.Location = location;
            panel.Size = new Size(235, 586);

            int[] buttonY = { 6, 155, 305, 458 };
            int[] labelY = { 71, 223, 370, 522 };

            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Size = new Size(200, 120);
                buttons[i].Location = new Point(15, buttonY[i]);
                buttons[i].Font = new Font("Roboto Condensed", 12f);

                labels[i].Font = new Font("Roboto Condensed", 20.25f);
                labels[i].Location = new Point(65, labelY[i]); // was 86
            }
        }
    }
}