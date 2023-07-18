﻿/*
Copyright (c) 2018, Lars Brubaker, John Lewin
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those
of the authors and should not be interpreted as representing official policies,
either expressed or implied, of the FreeBSD Project.
*/

using MatterHackers.Agg;
using MatterHackers.Agg.UI;

namespace MatterHackers.Agg.UI
{
	public class ThemedNumberEdit : GuiWidget
	{
		private ThemeConfig theme;

		public NumberEdit ActuallNumberEdit { get; }

		public ThemedNumberEdit(double startingValue, ThemeConfig theme, char singleCharLabel = char.MaxValue, string unitsLabel = "", double pixelWidth = 0, double pixelHeight = 0, bool allowNegatives = false, bool allowDecimals = false, double minValue = int.MinValue, double maxValue = int.MaxValue, double increment = 1, int tabIndex = 0)
		{
			using (this.LayoutLock())
			{
				this.Padding = 3;
				this.HAnchor = HAnchor.Fit;
				this.VAnchor = VAnchor.Fit;
				this.Border = 1;
				this.theme = theme;

				this.ActuallNumberEdit = new NumberEdit(startingValue, 0, 0, theme.DefaultFontSize, pixelWidth, pixelHeight, allowNegatives, allowDecimals, minValue, maxValue, increment, tabIndex)
				{
					VAnchor = VAnchor.Bottom,
				};

				TextWidget labelWidget = null;
				if (singleCharLabel != char.MaxValue)
				{
					labelWidget = new TextWidget(singleCharLabel.ToString(), pointSize: theme.DefaultFontSize - 2, textColor: theme.PrimaryAccentColor)
					{
						Margin = new BorderDouble(left: 2),
						HAnchor = HAnchor.Left,
						VAnchor = VAnchor.Center,
						Selectable = false
					};

					this.AddChild(labelWidget);

					var labelWidth = labelWidget.Width + labelWidget.Margin.Left;
					ActuallNumberEdit.Margin = ActuallNumberEdit.Margin.Clone(left: labelWidth + 2);
				}

				var internalWidget = this.ActuallNumberEdit.InternalTextEditWidget;
				internalWidget.TextColor = theme.EditFieldColors.Inactive.TextColor;
				internalWidget.FocusChanged += (s, e) =>
				{
					internalWidget.TextColor = (internalWidget.Focused) ? theme.EditFieldColors.Focused.TextColor : theme.EditFieldColors.Inactive.TextColor;

					if (labelWidget != null)
					{
						var labelDetailsColor = theme.PrimaryAccentColor.WithContrast(theme.EditFieldColors.Focused.BackgroundColor, 3).ToColor();
						labelWidget.TextColor = (internalWidget.Focused) ? labelDetailsColor : theme.PrimaryAccentColor;
					}
				};

				this.ActuallNumberEdit.InternalNumberEdit.MaxDecimalsPlaces = 5;
				this.AddChild(this.ActuallNumberEdit);
			}

			this.PerformLayout();
		}

		public override Color BackgroundColor
		{
			get
			{
				if (base.BackgroundColor != Color.Transparent)
				{
					return base.BackgroundColor;
				}
				else if (this.ContainsFocus)
				{
					return theme.EditFieldColors.Focused.BackgroundColor;
				}
				else if (this.mouseInBounds)
				{
					return theme.EditFieldColors.Hovered.BackgroundColor;
				}
				else
				{
					return theme.EditFieldColors.Inactive.BackgroundColor;
				}
			}
			set => base.BackgroundColor = value;
		}

		public override Color BorderColor
		{
			get
			{
				if (base.BorderColor != Color.Transparent)
				{
					return base.BorderColor;
				}
				else if (this.ContainsFocus)
				{
					return theme.EditFieldColors.Focused.BorderColor;
				}
				else if (this.mouseInBounds && this.ContainsFirstUnderMouseRecursive())
				{
					return theme.EditFieldColors.Hovered.BorderColor;
				}
				else
				{
					return theme.EditFieldColors.Inactive.BorderColor;
				}
			}
			set => base.BorderColor = value;
		}

		private bool mouseInBounds = false;

		public override void OnMouseEnterBounds(MouseEventArgs mouseEvent)
		{
			mouseInBounds = true;
			base.OnMouseEnterBounds(mouseEvent);

			this.Invalidate();
		}

		public override void OnMouseLeaveBounds(MouseEventArgs mouseEvent)
		{
			mouseInBounds = false;
			base.OnMouseLeaveBounds(mouseEvent);

			this.Invalidate();
		}

		public override int TabIndex
		{
			// TODO: This looks invalid - setter and getter should use same context
			get => base.TabIndex;
			set => this.ActuallNumberEdit.TabIndex = value;
		}

		public double Value
		{
			get => this.ActuallNumberEdit.Value;
			set => this.ActuallNumberEdit.Value = value;
		}

		public override string Text
		{
			get => this.ActuallNumberEdit.Text;
			set => this.ActuallNumberEdit.Text = value;
		}

		public bool SelectAllOnFocus
		{
			get => this.ActuallNumberEdit.InternalNumberEdit.SelectAllOnFocus;
			set => this.ActuallNumberEdit.InternalNumberEdit.SelectAllOnFocus = value;
		}
	}
}