using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Sitecore.Web.UI
{
    using System.Web.UI.WebControls;

    using Sitecore.Diagnostics;
    using Sitecore.LogWatcher.Services;

    public class LogMessageViewer : Page
    {
        protected GridView ResultGrid;

        /// <summary>
        /// Gets or sets the sort column.
        /// </summary>
        /// <value>The sort column.</value>
        protected string SortColumn
        {
            get
            {
                if (Session["sort_column"] == null)
                {
                    return string.Empty;
                }

                return Session["sort_column"].ToString();
            }

            set
            {
                Session["sort_column"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        /// <value>The contingency entries.</value>
        protected List<LogWatcher.MessageDetails> Entries
        {
            get
            {
                return Session["entries"] as List<LogWatcher.MessageDetails>;
            }

            set
            {
                Session["entries"] = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.LoadEntries();
            this.InitializePageControls(this.Entries);
        }

        /// <summary>
        /// Initializes the page controls.
        /// </summary>
        /// <param name="entries">The message entries.</param>
        protected void InitializePageControls(List<LogWatcher.MessageDetails> entries)
        {
            this.SortColumn = null;
            this.BindGrid(entries);
        }

        protected virtual void LoadEntries()
        {
            var entries = this.GetEntries();
            this.ProcessEntries(entries);
            this.Entries = entries;
        }

        public List<LogWatcher.MessageDetails> GetEntries()
        {
            return LogWatcher.GetMessages();
        }

        protected virtual void ProcessEntries(List<LogWatcher.MessageDetails> entries)
        {
            if (entries == null)
            {
                return;
            }

            foreach (var entry in entries)
            {
                entry.Message = this.ProcessDataChunks(entry.Message);
            }
        }

        protected virtual string ProcessDataChunks(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            return text.Replace("[s]", "<span onclick='javascript:SetSelection(this);'>").Replace("[/s]", "</span>");
        }

        /// <summary>
        /// Gets the index of the column to be sorted.
        /// </summary>
        /// <returns>The column number.</returns>
        protected int GetSortColumnIndex()
        {
            if (!string.IsNullOrEmpty(this.SortColumn))
            {
                foreach (DataControlField field in ResultGrid.Columns)
                {
                    if (string.Compare(field.SortExpression, this.SortColumn, true) == 0)
                    {
                        return ResultGrid.Columns.IndexOf(field);
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        /// <param name="name">The sorting column name.</param>
        /// <returns>The Sort direction.</returns>
        protected SortDirection GetDirection(string name)
        {
            if (Session[name + "_sorting"] == null)
            {
                Session[name + "_sorting"] = SortDirection.Ascending;
            }

            return (SortDirection)Session[name + "_sorting"];
        }

        /// <summary>
        /// Sets the direction.
        /// </summary>
        /// <param name="name">The name of sorting column.</param>
        /// <param name="direction">The direction.</param>
        protected void SetDirection(string name, SortDirection direction)
        {
            Session[name + "_sorting"] = direction;
            this.SortColumn = name;
        }

        /// <summary>
        /// Adds the sorting image to the sorting column.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="headerRow">The DataGrid header row.</param>
        protected void AddSortImage(int columnIndex, GridViewRow headerRow)
        {
            Image sortImage = new Image();
            if (this.GetDirection(this.SortColumn) == SortDirection.Ascending)
            {
                sortImage.ImageUrl = "~/sitecore/shell/Themes/Standard/Images/sortdown9x5.png";
                sortImage.AlternateText = "Ascending Order";
            }
            else
            {
                sortImage.ImageUrl = "~/sitecore/shell/Themes/Standard/Images/sortup9x5.png";
                sortImage.AlternateText = "Descending Order";
            }

            sortImage.Style.Add("align", "center");
            sortImage.Style.Add("valign", "center");
            headerRow.Cells[columnIndex].Controls.Add(sortImage);
        }

        /// <summary>
        /// Handles the RowCreated event of the ResultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
        protected void ResultGrid_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                int sortColumnIndex = this.GetSortColumnIndex();

                if (sortColumnIndex != -1)
                {
                    this.AddSortImage(sortColumnIndex, e.Row);
                }
            }
        }

        /// <summary>
        /// Handles the Sorted event of the ResultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ResultGrid_Sorted(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the Sorting event of the ResultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewSortEventArgs"/> instance containing the event data.</param>
        protected void ResultGrid_Sorting(object sender, GridViewSortEventArgs e)
        {
            List<LogWatcher.MessageDetails> entries = this.Entries;
            if (entries == null)
            {
                e.Cancel = true;
                return;
            }

            IComparer<LogWatcher.MessageDetails> comparer = new MessageDetailsComparer(e.SortExpression);

            entries.Sort(comparer);
            if (this.GetDirection(e.SortExpression.ToLowerInvariant()) == SortDirection.Descending)
            {
                entries.Reverse();
                this.SetDirection(e.SortExpression.ToLowerInvariant(), SortDirection.Ascending);
            }
            else
            {
                this.SetDirection(e.SortExpression.ToLowerInvariant(), SortDirection.Descending);
            }

            this.BindGrid(entries);
        }

        /// <summary>
        /// Binds the data grid.
        /// </summary>
        /// <param name="entries">The messages.</param>
        protected void BindGrid(List<LogWatcher.MessageDetails> entries)
        {
            List<LogWatcher.MessageDetails> source = entries;
            if (source.Count == 0)
            {
                ResultGrid.EmptyDataText = string.Format("No messages found.");
            }

            ResultGrid.DataSource = source;
            ResultGrid.DataBind();
        }

        /// <summary>
        /// Handles the RowDataBound event of the ResultGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
        protected void ResultGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                {
                    LogWatcher.MessageDetails entry = e.Row.DataItem as LogWatcher.MessageDetails;
                    if (entry != null)
                    {
                        Label description = e.Row.FindControl("Description") as Label;
                        if (description != null)
                        {
                            if (entry.Exception != null)
                            {
                                description.Text = this.ProcessDataChunks(entry.Exception.ToString());
                            }
                            else
                            {
                                description.Text = this.ProcessDataChunks(entry.Message);
                            }
                        }

                        Label shortDescription = e.Row.FindControl("ShortDescription") as Label;
                        if (shortDescription != null)
                        {
                            shortDescription.Text = entry.Message;
                        }

                        if ((description != null) && (shortDescription != null))
                        {
                            shortDescription.Attributes.Add("onclick", "javascript:showHideDescription('" + description.ClientID + "')");
                        }

                        Label messageType = e.Row.FindControl("MessageType") as Label;
                        if ((messageType != null) && (messageType.Parent is DataControlFieldCell))
                        {
                            DataControlFieldCell ctl = messageType.Parent as DataControlFieldCell;
                            switch (entry.Level)
                            {
                                case LogNotificationLevel.Fatal:
                                    ctl.CssClass = "message collision";
                                    break;
                                case LogNotificationLevel.Error:
                                    ctl.CssClass = "message error";
                                    break;
                                case LogNotificationLevel.Info:
                                case LogNotificationLevel.Debug:
                                case LogNotificationLevel.None:
                                    ctl.CssClass = "message info";
                                    break;
                                case LogNotificationLevel.Warning:
                                    ctl.CssClass = "message warning";
                                    break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The contingency entry comparer.
        /// </summary>
        public class MessageDetailsComparer : IComparer<LogWatcher.MessageDetails>
        {
            #region Fields

            /// <summary>
            /// The field name.
            /// </summary>
            private readonly string fieldName;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ContingencyEntryComparer"/> class.
            /// </summary>
            /// <param name="fieldName">Name of the field.</param>
            public MessageDetailsComparer(string fieldName)
            {
                this.fieldName = fieldName;
            }

            #endregion

            #region Public methods

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>
            /// Value
            /// Condition
            /// Less than zero
            /// <paramref name="x"/> is less than <paramref name="y"/>.
            /// Zero
            /// <paramref name="x"/> equals <paramref name="y"/>.
            /// Greater than zero
            /// <paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            public int Compare(LogWatcher.MessageDetails x, LogWatcher.MessageDetails y)
            {
                string val1 = x.GetType().GetProperty(this.fieldName).GetValue(x, new object[] { }).ToString();
                string val2 = y.GetType().GetProperty(this.fieldName).GetValue(y, new object[] { }).ToString();
                if (x.GetType().GetProperty(this.fieldName).PropertyType.UnderlyingSystemType == typeof(DateTime))
                {
                    try
                    {
                        DateTime propertyValue1 = DateTime.Parse(val1);
                        DateTime propertyValue2 = DateTime.Parse(val2);
                        return propertyValue1.CompareTo(propertyValue2);
                    }
                    catch
                    {
                    }
                }

                return string.Compare(val1, val2);
            }

            #endregion
        }
    }
}
