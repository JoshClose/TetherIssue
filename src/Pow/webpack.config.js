﻿const path = require("path");
const webpack = require("webpack");
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const merge = require("webpack-merge");

module.exports = (env) => {
	const isDevBuild = !(env && env.prod);

	const sharedConfig = () => ({
		stats: { modules: false },
		resolve: { extensions: [".js"] },
		output: {
			filename: "[name].js",
			publicPath: "/dist/"
		},
		module: {
			rules: [{
				test: /\.js$/,
				include: /ClientApp/,
				exclude: /node_modules/,
				use: {
					loader: "babel-loader",
					options: {
						presets: ["latest", "stage-2", "react"]
					}
				}
			}]
		}
	});

	const clientBundleOutputDir = "./wwwroot/dist";
	const clientBundleConfig = merge(sharedConfig(), {
		entry: { "client": "./ClientApp/client.js" },
		module: {
			rules: [
				{ test: /\.css$/, use: ExtractTextPlugin.extract({ use: isDevBuild ? "css-loader" : "css-loader?minimize" }) },
				{ test: /\.(png|jpg|jpeg|gif|svg)$/, use: "url-loader?limit=25000" }
			]
		},
		output: {
			path: path.join(__dirname, clientBundleOutputDir)
		},
		plugins: [
			new ExtractTextPlugin("site.css"),
			new webpack.DllReferencePlugin({
				context: __dirname,
				manifest: require("./wwwroot/dist/vendor-manifest.json")
			})
		].concat(isDevBuild ? [
			// Plugins that apply in development builds only
			new webpack.SourceMapDevToolPlugin({
				filename: "[file].map", // Remove this line if you prefer inline source maps
				moduleFilenameTemplate: path.relative(clientBundleOutputDir, "[resourcePath]") // Point sourcemap entries to the original file locations on disk
			})
		] : [
			// Plugins that apply in production builds only
			new webpack.optimize.UglifyJsPlugin()
		])
	});

	const serverBundleConfig = merge(sharedConfig(), {
		resolve: { mainFields: ["main"] },
		entry: { "server": "./ClientApp/server.js" },
		plugins: [
			new webpack.DllReferencePlugin({
				context: __dirname,
				manifest: require("./ClientApp/dist/vendor-manifest.json"),
				sourceType: "commonjs2",
				name: "./vendor"
			})
		],
		output: {
			libraryTarget: "commonjs",
			path: path.join(__dirname, "./ClientApp/dist")
		},
		target: "node",
		devtool: "inline-source-map"
	});

	return [clientBundleConfig, serverBundleConfig];
};