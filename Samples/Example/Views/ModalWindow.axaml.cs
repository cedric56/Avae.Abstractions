using Avae.Avalonia;
using Avae.Services;
using Example.ViewModels;
using System;

namespace Example;

public partial class ModalWindow : DialogView<ModalViewModel, string?>
{
    private Guid id = Guid.NewGuid();

    protected override bool IsFluent => true;
    protected override string Icon => "avares://Example/Assets/avalonia-logo.ico";

    public ModalWindow(IContentDialogService contentDialogService)
        : base(contentDialogService)
    {
        InitializeComponent();
        tb.Text = id.ToString();
    }
}