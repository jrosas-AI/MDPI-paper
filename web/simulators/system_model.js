
var variableStates = {};





var variableStates = {};
var sendVariablesToServerMutex = false;
var isFetching = false;

function sendVariableToServer(variableName, value) {
    // Atualiza o estado da vari�vel
    variableStates[variableName] = value;

    // Verifica se o mutex est� ativo para evitar chamadas concorrentes
    if (!sendVariablesToServerMutex) {
        sendVariablesToServerMutex = true;

        fetch('/postVariables', {
            method: 'POST', // Especifica o m�todo HTTP
            headers: {
                'Content-Type': 'application/json' // Especifica o tipo de conte�do
            },
            body: JSON.stringify(variableStates) // Converte os dados para uma string JSON
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.text(); // Retorna o texto da resposta
            })
            .then(data => {
                console.log(data);
                sendVariablesToServerMutex = false;
            })
            .catch(error => {
                sendVariablesToServerMutex = false;
                console.error('Error calling sendVariablesToServer:', error);
            });
    }
}

function getVariables() {
    if (!isFetching && !sendVariablesToServerMutex) {
        isFetching = true;
        fetch('/getVariables')
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                variableStates = data;
                isFetching = false;
            })
            .catch(error => {
                console.error('Error fetching data:', error);
                isFetching = false;
            });
    }
}

