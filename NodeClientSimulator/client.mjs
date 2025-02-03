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
                    callbackOnObj(data.toString('utf8'))
                })
            },
            onClose: function(callback) {
                client.on('close', callback)
            }
        })
    });

}

connectTo(5000, '127.0.0.1', client => {
    client.onReceive(obj => {
        console.log(obj)
    })
    client.send({
        name: "SetUsername",
        data: [{ "data": "daverave1212" }]
    })
    client.send({
        name: "Say",
        data: [{ "data": "gorgodanc" }]
    })
    client.send({
        name: "Say",
        data: [{ "data": "krantz" }]
    })
    // client.send({
    //     name: "Dave",
    //     age: 26,
    //     computer: "Pentium 2"
    // })
    // client.send({
    //     ALGORITHM: "NONE",
    //     SALT: ":3"
    // })
    // client.send({
    //     name: "Say",
    //     data: [{ "data": "Hello sevver!" }]
    // })
    // setTimeout(() => {
    //     client.send({
    //         name: "Say",
    //         data: [{ "data": "Apples are awesome!" }]
    //     })
    // }, 3500)
    // setTimeout(() => {
    //     client.send({
    //         name: "Say",
    //         data: [{ "data": "Tesla is very cool!" }]
    //     })
    // }, 4500)
})

// setTimeout(() => {
//     connectTo(8080, '127.0.0.1', client => {
//         client.onReceive(obj => {
            
//         })
//         // client.send({name: "Dave"})
//         // client.send({kurwa: "cyka"})
//         client.send({
//             name: "SetUsername",
//             data: [{ "data": "Prangundha" }]
//         })
//         client.send({
//             name: "Say",
//             data: [{ "data": "Sadhanna ho gae?" }]
//         })
//     })
// }, 4000)


