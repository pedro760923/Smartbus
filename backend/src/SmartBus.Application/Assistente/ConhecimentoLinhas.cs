namespace SmartBus.Application.Assistente;

/// <summary>
/// Conhecimento estático sobre linhas da EMTU que não estão cadastradas
/// no banco do SmartBus (069 e 323) — transcrito/condensado a partir de
/// pesquisas oficiais da EMTU (emtu.sp.gov.br), consulta em 24/10/2026.
/// Injetado como contexto no prompt do assistente (RAG simples: o corpus
/// é pequeno o bastante pra caber inteiro no contexto, não precisa de
/// busca vetorial/embeddings).
/// </summary>
public static class ConhecimentoLinhas
{
    public const string Linha069 = """
        LINHA 069 — Santo André (Terminal Metropolitano Santo André Leste) ↔ São Paulo (Fábrica Trol / Rua Tocantínia)
        Via São Bernardo do Campo (Rudge Ramos). Operadora: NextMobilidade (ABC Sistema de Transporte SPE S.A.). Serviço: Comum.
        Tarifa autorizada: R$ 4,55 (integrada com trilhos/Metrô-CPTM: R$ 8,45; com vale-transporte: R$ 8,97).
        Tempo de percurso: ~35 min (Santo André → São Paulo), ~40 min (São Paulo → Santo André).

        Partidas de Santo André (Terminal Leste):
        - Dias úteis: 04:20 às 00:05, cerca de 65 partidas/dia. Intervalo mais curto (~12 min) entre 06:00 e 07:00; ~15-20 min na maior parte do dia; passa a ~20-30 min após as 21:00.
        - Sábados: 04:25 às 23:40.
        - Domingos e feriados: 04:25 às 23:15.

        Partidas de São Paulo (Fábrica Trol / Rua Tocantínia):
        - Dias úteis: 04:45 às 23:30.
        - Sábados: 04:55 às 23:20.
        - Domingos e feriados: 04:50 às 23:35.

        Principais vias no trajeto (sentido Santo André → São Paulo): Terminal Metropolitano Santo André Leste, Rua Augusto Ruschi, Rua João Pires Gago, Avenida Antônio Cardoso, Viaduto Pedro Dell'Antônia, Avenida Industrial, Rua São Vicente, Rua Catequese, Praça IV Centenário, Praça Monte Cristo, Avenida Príncipe de Gales, Praça São Jorge, Fundação Santo André, Avenida Prestes Maia, Viaduto Engenheiro Luís Meira, Avenida Novo Horizonte, Rua Lauro Muller, Rua Afonsina, Avenida Senador Vergueiro, Avenida Doutor Rudge Ramos (São Bernardo do Campo), Rua Tocantínia (São Paulo). O sentido contrário passa pelas mesmas vias principais, na ordem inversa.
        """;

    public const string Linha323 = """
        LINHA 323 — Santo André (Terminal Metropolitano Santo André Leste) ↔ São Paulo (Jardim Clímax / Rua José Pereira Barreto)
        Via São Bernardo do Campo (Rudge Ramos) e São Paulo (região de Vila Livieiro/Capão Redondo). Operadora: NextMobilidade (ABC Sistema de Transporte SPE S.A.). Serviço: Comum.
        Tarifa autorizada: R$ 5,90 (integrada com trilhos/Metrô-CPTM: R$ 9,80; com vale-transporte: R$ 10,32).
        Tempo de percurso: ~60 min em cada sentido (linha mais longa que a 069).

        Partidas de Santo André (Terminal Leste):
        - Dias úteis: 04:20 às 23:10, intervalos mais espaçados que a 069 (de ~20 a ~45 min dependendo do horário).
        - Sábados: 04:20 às 23:10.
        - Domingos e feriados: 04:20 às 23:10.

        Partidas de São Paulo (Jardim Clímax):
        - Dias úteis: 05:00 às 23:20.
        - Sábados: 05:10 às 23:20.
        - Domingos e feriados: 05:10 às 23:20.

        Principais vias no trajeto (sentido Santo André → São Paulo): Terminal Metropolitano Santo André Leste, Avenida Industrial, Praça Monte Cristo, Avenida Príncipe de Gales, Fundação Santo André, Avenida Prestes Maia, Viaduto Engenheiro Luís Meira, Avenida Novo Horizonte, Avenida Gago Coutinho, Praça Lisboa, Avenida Carlos Gomes, Praça Tangará, Avenida Palmares, Rua Lauro Muller, Rua Afonsina, Avenida Senador Vergueiro, Avenida Doutor Rudge Ramos, Avenida do Taboão (São Bernardo do Campo), trecho por bairros da zona sul de São Paulo (Capão Redondo/Vila Clímax), terminando no Jardim Clímax (Rua José Pereira Barreto). O sentido contrário passa pelas mesmas vias principais, na ordem inversa.
        """;
}
