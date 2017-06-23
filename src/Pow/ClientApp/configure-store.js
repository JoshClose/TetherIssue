import { createStore, applyMiddleware, compose, combineReducers, GenericStoreEnhancer, Store } from "redux"
import thunk from "redux-thunk"
import { routerReducer, routerMiddleware } from "react-router-redux"
import reducers from "./reducers"

export default function configureStore(history, initialState, logger) {
	// Build middleware. These are functions that can process the actions before they reach the store.
	const windowIsDefined = typeof window === "undefined" ? null : window;

	// If devTools is installed, connect to it
	const devToolsExtension = windowIsDefined && windowIsDefined.devToolsExtension;
	const devToolsExtensionFunction = devToolsExtension ? devToolsExtension() : f => f;
	const store = createStore(reducers, applyMiddleware(thunk, routerMiddleware(history)), devToolsExtensionFunction, logger, initialState);

	// Enable Webpack hot module replacement for reducers
	if (module.hot) {
		module.hot.accept("./reducers", () => {
			const newReducers = require("./reducers");
			store.replaceReducer(newReducers);
		});
	}

	return store;
}