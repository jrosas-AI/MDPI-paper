from http.server import BaseHTTPRequestHandler, HTTPServer
import os
import json
import urllib.parse as urlparse

variableStates = {}  # with values eg variableStates["s1"]="1", variableStates["s2"]="0"
variableStatesBefore = {}  # with values eg variableStates["s1"]="0", variableStates["s2"]="0"



def getVariableValue(name):
    if name in variableStates:
        return variableStates[name]
    else:
        return None


def handleInterrupts():
    # handle interrupts       
    for variableName in variableStates:
        valueNow = variableStates[variableName]
        valueBefore = variableStatesBefore.get(variableName, None)
        
        # if valueBefore is not None and valueBefore != valueNow:
        #    variableValueChanged(variableName, valueBefore, valueNow)

def variableValueChanged(variableName, valueBefore, valueNow):
    if valueBefore == "0" and valueNow == "1":
        print("rising edge")
    elif valueBefore == "1" and valueNow == "0":
        print("falling edge")
    else:
        print(f"value changed: {variableName}, before: {valueBefore}, now:{valueNow}...")

class MyServer(BaseHTTPRequestHandler):

    def log_message(self, format, *args):
        # Suppress logging of HTTP request messages
        pass

    def do_GET(self):
        parsed_path = urlparse.urlparse(self.path)
        if parsed_path.path == '/getVariables':
            # Send the response with variableStates
            response_data = json.dumps(variableStates)  # Convert variableStates to JSON string
            self.send_response(200)
            self.send_header('Content-type', 'application/json')  # Set content type to JSON
            self.end_headers()
            self.wfile.write(response_data.encode('utf-8'))  # Write the response data
        elif parsed_path.path == '/setVariable':
            # Parse query parameters
            query_components = urlparse.parse_qs(parsed_path.query)
            variableName = query_components.get("variableName", [None])[0]
            value = query_components.get("value", [None])[0]
            
            if variableName and value is not None:
                # Update the variableStates dictionary
                variableStatesBefore[variableName] = variableStates.get(variableName, "0")
                variableStates[variableName] = value

                # Handle interrupts
                handleInterrupts()

                # _HERE_ should return the same result as the request "/getVariables"
                response_data = json.dumps(variableStates)  # Convert variableStates to JSON string
                self.send_response(200)
                self.send_header('Content-type', 'application/json')  # Set content type to JSON
                self.end_headers()
                self.wfile.write(response_data.encode('utf-8'))  # Write the response data
            else:
                self.send_response(400)
                self.send_header('Content-type', 'text/plain')
                self.end_headers()
                self.wfile.write(b"Missing variableName or value parameter")
        else:
            # Serve static files from the 'simulators' directory
            filename = os.path.join("simulators", self.path[1:])
            if os.path.exists(filename) and os.path.isfile(filename):
                self.send_response(200)
                if filename.endswith('.html'):
                    self.send_header('Content-type', 'text/html')
                elif filename.endswith('.js'):
                    self.send_header('Content-type', 'application/javascript')
                elif filename.endswith('.css'):
                    self.send_header('Content-type', 'text/css')
                elif filename.endswith('.wasm'):
                    self.send_header('Content-type', 'application/wasm')
                elif filename.endswith('.gz'):
                    self.send_header('Content-type', 'application/wasm')
                    self.send_header('Content-Encoding', 'gzip')
                else:
                    self.send_header('Content-type', 'text/plain')
                self.end_headers()
                # Stream the file in chunks
                chunk_size = 4096
                with open(filename, 'rb') as file:
                    while True:
                        chunk = file.read(chunk_size)
                        if not chunk:
                            break
                        self.wfile.write(chunk)
            else:
                self.send_response(404)
                self.send_header('Content-type', 'text/plain')
                self.end_headers()
                self.wfile.write(bytes("Not found", "utf-8"))

    def do_POST(self):
        parsed_path = urlparse.urlparse(self.path)
        if parsed_path.path == '/postVariables':
            # Get the length of the request body
            content_length = int(self.headers['Content-Length'])
            # Read the request body
            post_data = self.rfile.read(content_length)
            # Parse the JSON data
            json_data = json.loads(post_data)
            
            global variableStates  # Declare as global to modify the global variable
            variableStates = json_data

            handleInterrupts()

            # Send the response with updated variableStates
            response_data = json.dumps(variableStates)  # Convert variableStates to JSON string
            self.send_response(200)
            self.send_header('Content-type', 'application/json')  # Set content type to JSON
            self.end_headers()
            self.wfile.write(response_data.encode('utf-8'))  # Write the response data
        elif parsed_path.path == '/setVariable':
            # Get the length of the request body
            content_length = int(self.headers['Content-Length'])
            # Read the request body
            post_data = self.rfile.read(content_length)
            # Parse the JSON data
            json_data = json.loads(post_data)
            variableName = json_data.get("variableName")
            value = json_data.get("value")

            if variableName and value is not None:
                # Update the variableStates dictionary
                variableStatesBefore[variableName] = variableStates.get(variableName, "0")
                variableStates[variableName] = value

                # Handle interrupts
                handleInterrupts()

                # _HERE_ should return the same result as the request "/getVariables"
                # print(str(variableStates))
                response_data = json.dumps(variableStates)  # Convert variableStates to JSON string
                self.send_response(200)
                self.send_header('Content-type', 'application/json')  # Set content type to JSON
                self.end_headers()
                self.wfile.write(response_data.encode('utf-8'))  # Write the response data
            else:
                self.send_response(400)
                self.send_header('Content-type', 'text/plain')
                self.end_headers()
                self.wfile.write(b"Missing variableName or value parameter")
        else:
            self.send_error(501, "Unsupported method ('POST')")

def run_server(address, port):
    server_address = (address, port)
    httpd = HTTPServer(server_address, MyServer)
    print('Starting server on port 8089...')
    httpd.serve_forever()

if __name__ == "__main__":
    run_server('127.0.0.1', 8089)
