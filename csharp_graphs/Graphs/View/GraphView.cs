using System;
using System.Drawing;
using Graphs.Presenter;
using Graphs.View.UI;
using Graphs.View.UI.Dialogs;
using SpaceVIL;
using SpaceVIL.Core;
using SpaceVIL.Decorations;

namespace Graphs.View
{
    public interface IGraphView
    {
        void SetPresenter(IGraphPresenter presenter);
        void SetGraphImage(string path);
        void AppendToLog(string logEntry);
    }
    
    public class GraphView : ActiveWindow, IGraphView
    {
        private IGraphPresenter _presenter;
        private VerticalStack _buttonsList;
        private TabView _tabView;
        private TextArea _logTextArea;
        private ImageItem _imageArea;
        private HorizontalStack _toolbar;
        
        public GraphView()
        {
            SetPresenter(new GraphPresenter(this));
        }
        
        public override void InitWindow()
        {
            SetParameters(
                nameof(GraphView),
                nameof(GraphView),
                800,
                800,
                false);
            SetMinSize(1000, 1000);
            SetBackground(45, 45, 45);

            var title = UiElements.GetTitleBar();
            var mainLayout = UiElements.GetMainLayout();
            var buttonsList = UiElements.GetButtonsList();
            _toolbar = UiElements.GetToolbar();
            var leftLayoutItem = UiElements.GetLeftLayoutItem();
            var tabView = UiElements.GetTabView();
            var logTextArea = UiElements.GetLogTextArea();
            
            _imageArea = new ImageItem(
                new Bitmap(
                    "/home/nick/RiderProjects/DiscreteMaths/Graphs/Graphs/notGenerated.png"));
            _imageArea.SetMaxWidth(1024);
            _imageArea.SetMaxHeight(1024);
            _imageArea.KeepAspectRatio(true);
            
            _tabView = tabView;
            _buttonsList = buttonsList;
            _logTextArea = logTextArea;
            _logTextArea.SetText("Log Journal" + Environment.NewLine);
            _logTextArea.SetFont(new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold));
            
            AddItems(title, mainLayout);
            mainLayout.AssignLeftItem(leftLayoutItem);
            mainLayout.AssignRightItem(buttonsList);
            leftLayoutItem.AddItems(tabView, _toolbar);
            
            tabView.AddTabs(new Tab("Graph"), new Tab("Log"));
            
            tabView.AddItemToTabByName("Log", _logTextArea);
            tabView.AddItemToTabByName("Graph", _imageArea);
            
            PopulateButtonsList();
            PopulateToolbar();
        }
        
        public void SetPresenter(IGraphPresenter presenter)
        {
            _presenter = presenter;
        }

        public void SetGraphImage(string path)
        {
            _imageArea.SetImage(new Bitmap(path));
        }

        private void PopulateButtonsList()
        {
            var generateButton = UiElements.GetButton("Generate Graph");
            generateButton.EventMouseClick += (sender, args) =>
            {
                var dialog = new GenerateGraphDialog(_presenter.MaxVertices);
                dialog.GotCorrectInput += OnGotGraphGenerationInput;
                dialog.Show(this);
            };

            var shimbelButton = UiElements.GetButton("Shimbel's Algorithm");
            shimbelButton.EventMouseClick += (sender, args) =>
            {
                if (_presenter.IsGraphGenerated())
                {
                    var dialog = new ShimbelDialog(_presenter.Vertices);
                    dialog.GotCorrectShimbel += OnGotShimbelInput;
                    dialog.Show(this);
                }
                else
                {
                    ShowPopUp("Generate graph first");
                }
            };

            var checkReachButton = UiElements.GetButton("Check Path Existence");
            checkReachButton.EventMouseClick += (sender, args) =>
            {
                if (_presenter.IsGraphGenerated())
                {
                    var dialog = new CheckPathExistenceDialog(_presenter.Vertices);
                    dialog.GotCorrectCheckPath += OnGotCheckPathInput;
                    dialog.Show(this);
                }
                else
                {
                    ShowPopUp("Generate graph first");
                }
            };

            var shortestPathButton = UiElements.GetButton("Find Shortest Path");
            shortestPathButton.EventMouseClick += (sender, args) =>
            {
                if (_presenter.IsGraphGenerated())
                {
                    var dialog = new ShortestPathDialog(_presenter.Vertices);
                    dialog.GotCorrectFindPath += OnGotFindPathInput;
                    dialog.Show(this);
                }
                else
                {
                    ShowPopUp("Generate graph first");
                }
            };

            var spanningTreesButton = UiElements.GetButton("Find Spanning Trees");
            spanningTreesButton.EventMouseClick += (sender, args) =>
            {
                if (_presenter.IsGraphGenerated())
                {
                    var dialog = new MinimumSpanningTreeDialog();
                    dialog.GotPrimRequest += OnGotPrimRequest;
                    dialog.GotKruskalRequest += OnGotKruskalRequest;
                    dialog.GotTotalRequest += OnGotTotalRequest;
                    dialog.Show(this);
                }
                else
                {
                    ShowPopUp("Generate graph first");
                }
            };

            var getPruferCode = UiElements.GetButton("Get Prufer Code for SST");
            getPruferCode.EventMouseClick += (sender, args) =>
            {
                if (_presenter.IsGraphGenerated())
                {
                    if (_presenter.IsSstGenerated())
                    {
                        ShowPopUp(_presenter.OnGetPruferCall());
                    }
                    else
                    {
                        ShowPopUp("Generate SST first");
                    }
                }
                else
                {
                    ShowPopUp("Generate graph first");
                }
            };

            var setPruferCode = UiElements.GetButton("Graph from Prufer Code");
            setPruferCode.EventMouseClick += (sender, args) =>
            {
                var dialog = new PruferCodeDialog();
                dialog.GotCorrectPruferInput += OnGotPruferInput;
                dialog.Show(this);
            };

            var generateFlowNetwork = UiElements.GetButton("Flow network from this");
            generateFlowNetwork.EventMouseClick += (sender, args) =>
            {
                if (_presenter.IsGraphGenerated())
                {
                    //_presenter.SaveGraph();
                    _presenter.OnCreateFlowNetworkCall();
                }
                else
                {
                    ShowPopUp("Generate graph first");
                }
            };

            var findMaxFlow = UiElements.GetButton("Find max flow");
            findMaxFlow.EventMouseClick += (sender, args) =>
            {
                if (_presenter.IsGraphGenerated())
                {
                    if (_presenter.IsFlowNetworkGenerated())
                    {
                        ShowPopUp("Max Flow: " + _presenter.OnMaxFlowCall().ToString());
                    }
                    else
                    {
                        ShowPopUp("Modify graph into flow network");
                    }
                }
                else
                {
                    ShowPopUp("Generate graph first");
                }
            };

            var findMinCostFlow = UiElements.GetButton("Find min cost flow");
            findMinCostFlow.EventMouseClick += (sender, args) =>
            {
                if (_presenter.IsGraphGenerated())
                {
                    if (_presenter.IsFlowNetworkGenerated())
                    {
                        ShowPopUp("Min Cost (2 * Max Flow / 3): " +
                                  _presenter.OnMinCostFlowCall().ToString());
                    }
                    else
                    {
                        ShowPopUp("Modify graph into flow network");
                    }
                }
                else
                {
                    ShowPopUp("Generate graph first");
                }
            };

            var checkEuler = UiElements.GetButton("Make Eulerian");
            var checkHamiltonian = UiElements.GetButton("Make Hamiltonian");

            _buttonsList.AddItems(
                generateButton,
                shimbelButton,
                checkReachButton,
                shortestPathButton,
                spanningTreesButton,
                getPruferCode,
                setPruferCode,
                generateFlowNetwork,
                findMaxFlow,
                findMinCostFlow,
                checkEuler,
                checkHamiltonian);
        }

        private void PopulateToolbar()
        {
            var resetGraph = UiElements.GetButton("Reset Graph");
            resetGraph.SetAlignment(Style.GetButtonCoreStyle().Alignment);
            resetGraph.EventMouseClick += (sender, args) =>
            {
                if (_presenter.IsGraphGenerated())
                {
                    _presenter.ResetGraph();
                    _presenter.SaveGraph();
                }
                else
                {
                    ShowPopUp("Generate graph first");
                }
            };
            
            _toolbar.AddItems(resetGraph);
        }

        private void OnGotTotalRequest(object sender, EventArgs e)
        {
            _presenter.OnTotalSstCall(sender as IPresenterConnectedDialog);
        }

        private void OnGotKruskalRequest(object sender, EventArgs e)
        {
            _presenter.OnKruskalAlgorithmCall(sender as IPresenterConnectedDialog);
        }

        private void OnGotPrimRequest(object sender, EventArgs e)
        {
            _presenter.OnPrimAlgorithmCall(sender as IPresenterConnectedDialog);
        }

        private void OnGotFindPathInput(object sender, GotCorrectFindPathEventArgs e)
        {
            _presenter.OnFindPathAlgorithmCall(
                sender as IPresenterConnectedDialog,
                e.FirstVertex,
                e.SecondVertex,
                e.Dijkstra,
                e.BellmanFord,
                e.Floyd);
        }

        private void OnGotGraphGenerationInput(object sender, GotCorrectInputEventArgs e)
        {
            _presenter.OnGraphPropertiesSet(e.VerticesAmount, e.Density, e.MinWeight, e.MaxWeight);
        }

        private void OnGotShimbelInput(object sender, GotCorrectShimbelEventArgs e)
        {
            _presenter.OnShimbelAlgorithmCall(sender as IPresenterConnectedDialog, e.EdgesAmount, e.ShortestPaths);
        }

        private void OnGotCheckPathInput(object sender, GotCorrectCheckPathEventArgs e)
        {
            _presenter.OnCheckPathAlgorithmCall(sender as IPresenterConnectedDialog, e.FirstVertex, e.SecondVertex);
        }

        private void OnGotPruferInput(object sender, GotCorrectPruferEventArgs e)
        {
            _presenter.OnDecodePruferCall(e.Code);
        }

        public void AppendToLog(string logEntry)
        {
            _logTextArea.SetEditable(true);
            _logTextArea.AppendText(logEntry);
            _logTextArea.SetEditable(false);
        }

        private void ShowPopUp(string message)
        {
            var popup = new PopUpMessage(message);
            popup.SetTimeOut(3000);
            popup.SetHeight(50);
            popup.Show(this);
        }
    }
}