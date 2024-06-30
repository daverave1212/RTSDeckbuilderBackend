import net from 'net';

function connectTo(port, ip, onConnect) {
    const client = new net.Socket();
    client.connect(port, ip, () => {
        console.log('Connected to server');
        onConnect({
            client,
            send: function(obj) {
                client.write(JSON.stringify(obj))
            },
            onReceive: function(callbackOnObj) {
                client.on('data', data => {
                    console.log(data)
                    callbackOnObj(data)
                })
            },
            onClose: function(callback) {
                client.on('close', callback)
            }
        })
    });

}

connectTo(8080, '127.0.0.1', client => {
    client.onReceive(obj => {
        
    })
    // client.send({name: "Dave"})
    // client.send({kurwa: "cyka"})
    client.send({
        name: "SetUsername",
        data: [{ "data": "daverave1212" }]
    })
    client.send({
        name: "Say",
        data: [{ "data": "Hello world!" }]
    })
    client.send({
        name: "Say",
        data: [{ "data": "Hello sevver!" }]
    })
    setTimeout(() => {
        client.send({
            name: "Say",
            data: [{ "data": "Apples are awesome!" }]
        })
    }, 3500)
    setTimeout(() => {
        client.send({
            name: "Say",
            data: [{ "data": "Tesla is very cool!" }]
        })
    }, 4500)
})

setTimeout(() => {
    connectTo(8080, '127.0.0.1', client => {
        client.onReceive(obj => {
            
        })
        // client.send({name: "Dave"})
        // client.send({kurwa: "cyka"})
        client.send({
            name: "SetUsername",
            data: [{ "data": "Prangundha" }]
        })
        client.send({
            name: "Say",
            data: [{ "data": "Sadhanna ho gae?" }]
        })
    })
}, 4000)





// setTimeout(() => {
//     const client2 = new net.Socket();
//     client2.connect(8080, '127.0.0.1', () => {
//         console.log('Connected to server');
//         setInterval(() => {
//             client2.write('    BOBR KURWA!!!');
//         }, 500)
//     });
//     client2.on('data', (data) => {
//         console.log('    (BOBR) Received:', data.toString());
//     });
//     client2.on('close', () => {});
// }, 1500)