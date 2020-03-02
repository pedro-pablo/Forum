function alterarVisibilidadeArquivos(postagem) {
    var postagemElement = document.getElementById(postagem);
    var arquivosPostElement = postagemElement.children[1].children[1];
    arquivosPostElement.style.display = arquivosPostElement.style.display === "none" ? "flex" : "none";
}

function enviarAjaxComentarios(id, elemento) {
    $.ajax({
        url: "/Postagens/ObterQuantidadeComentarios?postagemId=" + id,
        dataType: "text",
        success: (qtd) => {
            elemento.innerHTML = qtd;
        }
    });
}

function atualizarComentarios() {
    var postagens = document.getElementsByClassName("post");
    for (let i = 0; i < postagens.length; i++) {
        var postagem = postagens[i];
        var qtdComentariosElemento = postagem.getElementsByClassName("qtd-comentarios")[0];
        if (qtdComentariosElemento) {
            enviarAjaxComentarios(postagem.dataset.postId, qtdComentariosElemento);
        }
    }
}