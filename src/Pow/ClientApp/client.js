import "./css/site.css"
import "bootstrap"
import * as React from "react"
import { render } from "react-dom"
import { AppContainer } from "react-hot-loader"
import { Provider } from "react-redux"
import { ConnectedRouter } from "react-router-redux"
import { createBrowserHistory } from "history"
import { createLogger } from "redux-logger"

import configureStore from "./configure-store"
import r from "./routes"
let routes = r;

// Create browser history to use in the Redux store
const history = createBrowserHistory();
const logger = createLogger({
	collapsed: true
});

// Get the application-wide store instance, prepopulating with state from the server where available.
const initialState = window.initialReduxState;
const store = configureStore(history, initialState, logger);

function renderApp() {
	// This code starts up the React app when it runs in a browser. It sets up the routing configuration
	// and injects the app into a DOM element.
	render(
		<AppContainer>
			<Provider store={store}>
				<ConnectedRouter history={history} children={routes} />
			</Provider>
		</AppContainer>,
		document.getElementById("root")
	);
}

renderApp();

// Allow Hot Module Replacement
if (module.hot) {
	module.hot.accept("./routes", () => {
		routes = require("./routes").routes;
		renderApp();
	});
}