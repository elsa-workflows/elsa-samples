const path = require('path');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const destinationFolder = path.resolve(__dirname, '..', 'wwwroot');

module.exports = {
    mode: 'development', // Use 'production' for optimized builds
    entry: {
        designer: './src/designer.ts'
    },
    output: {
        filename: '[name].js',
        path: path.resolve(destinationFolder)
    },
    resolve: {
        extensions: ['.ts', '.js', '.tsx'], // Resolve these extensions
    },
    module: {
        rules: [
            {
                test: /\.ts$/, // For .ts files
                use: 'ts-loader', // Use ts-loader to transpile TypeScript
                exclude: /node_modules/
            }
        ]
    },
    plugins: [
        new CopyWebpackPlugin({
            patterns: [
                {
                    from: path.resolve(__dirname, 'node_modules/@elsa-workflows/elsa-studio-wasm'),
                    to: path.resolve(destinationFolder),
                },
            ],
        }),
    ],
    devtool: 'source-map', // Enable source maps
    devServer: {
        static: {
            directory: path.join(destinationFolder) // Serve files from 'wwwroot'
        },
        compress: true,
        port: 9000 // Start a dev server at localhost:9000
    }
};